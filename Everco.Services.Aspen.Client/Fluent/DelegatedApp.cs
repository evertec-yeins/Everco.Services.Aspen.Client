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

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance delegada.
    /// </summary>
    public sealed partial class DelegatedApp : AppBase<IDelegatedApp>, IDelegatedApp
    {
        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="DelegatedApp"/>
        /// </summary>
        private DelegatedApp()
        {
        }

        /// <summary>
        /// Obtiene una instancia que permite conectar con el servico Aspen.
        /// </summary>
        /// <returns>Instancia de <see cref="IRouting{TFluent}"/> que permite establecer la información de conexión.</returns>
        public static IRouting<IDelegatedApp> Initialize()
        {
            return new DelegatedApp();
        }

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="docNumber">El número de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="password">La clave de acceso del usuario que firma la solicitud de autenticación.</param>
        /// <param name="deviceInfo">La información del dispositivo desde donde se intenta autenticar el usuario.</param>
        /// <param name="useCache">Cuando es <see langword="true" /> se utiliza el último token de autenticación generado en la sesión.</param>
        /// <returns>
        /// Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.
        /// </returns>
        public ISession<IDelegatedApp> Authenticate(string docType, string docNumber, string password, IDeviceInfo deviceInfo = null, bool useCache = true)
        {
            StaticUserIdentity staticUserIdentity = new StaticUserIdentity(docType, docNumber, password, deviceInfo);
            return this.Authenticate(staticUserIdentity, useCache);
        }

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="userIdentity">La información de usuario que firma la solicitud de autenticación.</param>
        /// <param name="useCache">Cuando es <see langword="true" /> se utiliza el último token de autenticación generado en la sesión.</param>
        /// <returns>
        /// Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.
        /// </returns>
        /// <exception cref="AspenException">Se produce cuando el servicio Aspen genera una respuesta de error.</exception>
        public ISession<IDelegatedApp> Authenticate(IUserIdentity userIdentity, bool useCache = true)
        {
            if (useCache)
            {
                this.AuthToken = CacheStore.GetCurrentToken();
                if (this.AuthToken != null)
                {
                    return this;
                }
            }

            this.InitializeClient();
            IRestRequest request = new AspenRequest($"{Routes.DelegatedRoot}/{Routes.Auth.Signin}", Method.POST);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSigninPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret, userIdentity);
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            this.AuthToken = JsonConvert.DeserializeObject<UserAuthToken>(this.DecodeJwtResponse(response.Content));
            CacheStore.SetCurrentToken(this.AuthToken);
            return this;
        }

        /// <summary>
        /// Envía la solicitud al servicio Aspen.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se convierte la respuesta del servicio Aspen.</typeparam>
        /// <param name="request">Información de la solicitud.</param>
        /// <returns>Instancia de <typeparamref name="TResponse"/> con la información de respuesta del servicio Aspen.</returns>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        private TResponse Execute<TResponse>(IRestRequest request) where TResponse : class, new()
        {
            UserAuthToken userAuthToken = (UserAuthToken)this.AuthToken;
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret, userAuthToken.Token, userAuthToken.Username);
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response.StatusCode == HttpStatusCode.NoContent
                ? default
                : JsonConvert.DeserializeObject<TResponse>(response.Content);
        }
    }
}