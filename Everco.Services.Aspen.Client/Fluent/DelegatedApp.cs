// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-08-22 10:42 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Net;
    using Auth;
    using Internals;
    using Newtonsoft.Json;
    using RestSharp;
    using IDeviceInfo = Identity.IDeviceInfo;
    using IUserIdentity = Identity.IUserIdentity;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance delegada.
    /// </summary>
    public sealed partial class DelegatedApp : AppBase<IDelegatedApp>, IDelegatedApp
    {
        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="DelegatedApp"/>
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        private DelegatedApp(CachePolicy cachePolicy)
        {
            CacheStore.Policy = cachePolicy;
        }

        /// <summary>
        /// Obtiene una instancia que permite conectar con el servico Aspen.
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        /// <returns>Instancia de <see cref="IRouting{TFluent}"/> que permite establecer la información de conexión.</returns>
        public static IRouting<IDelegatedApp> Initialize(
            CachePolicy cachePolicy = CachePolicy.CacheIfAvailable)
        {
            return new DelegatedApp(cachePolicy);
        }

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="docNumber">El número de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="password">La clave de acceso del usuario que firma la solicitud de autenticación.</param>
        /// <param name="deviceInfo">La información del dispositivo desde donde se intenta autenticar el usuario.</param>
        /// <returns>
        /// Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.
        /// </returns>
        public ISession<IDelegatedApp> Authenticate(
            string docType,
            string docNumber,
            string password,
            IDeviceInfo deviceInfo = null)
        {
            StaticUserIdentity userIdentity = new StaticUserIdentity(docType, docNumber, password, deviceInfo);
            return this.Authenticate(userIdentity);
        }

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="userIdentity">La información de usuario que firma la solicitud de autenticación.</param>
        /// <returns>
        /// Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.
        /// </returns>
        public ISession<IDelegatedApp> Authenticate(IUserIdentity userIdentity)
        {
            Throw.IfNull(userIdentity, nameof(userIdentity));

            if (!ServiceLocator.Instance.Runtime.IsTestingExecuting)
            {
                this.AuthToken = CacheStore.Get<AuthToken>(CacheKeys.CurrentAuthToken);
                if (this.AuthToken != null)
                {
                    return this;
                }
            }
            
            this.InitializeClient();
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.Signin);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSigninPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret, userIdentity);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, null);
            IRestResponse response = base.Execute(request);
            this.AuthToken = JsonConvert.DeserializeObject<UserAuthToken>(this.DecodeJwtResponse(response.Content));
            CacheStore.Add(CacheKeys.CurrentAuthToken, this.AuthToken);
            return this;
        }

        /// <summary>
        /// Envía la solicitud al servicio Aspen.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se convierte la respuesta del servicio Aspen.</typeparam>
        /// <param name="request">Información de la solicitud.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        /// <returns>Instancia de <typeparamref name="TResponse"/> con la información de respuesta del servicio Aspen.</returns>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        internal TResponse Execute<TResponse>(IRestRequest request, string apiVersion = null) where TResponse : class, new()
        {
            UserAuthToken userAuthToken = (UserAuthToken)this.AuthToken;
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(
                request,
                this.JwtEncoder,
                this.AppIdentity.ApiSecret,
                userAuthToken.Token,
                userAuthToken.Username);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, apiVersion);
            IRestResponse response = base.Execute(request);
            return response.StatusCode == HttpStatusCode.NoContent
                ? default
                : JsonConvert.DeserializeObject<TResponse>(response.Content);
        }

        /// <summary>
        /// Envía la solicitud al servicio Aspen.
        /// </summary>
        /// <param name="request">Información de la solicitud.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        internal void Execute(IRestRequest request, string apiVersion = null)
        {
            UserAuthToken userAuthToken = (UserAuthToken)this.AuthToken;
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(
                request,
                this.JwtEncoder,
                this.AppIdentity.ApiSecret,
                userAuthToken.Token,
                userAuthToken.Username);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, apiVersion);
            base.Execute(request);
        }
    }
}