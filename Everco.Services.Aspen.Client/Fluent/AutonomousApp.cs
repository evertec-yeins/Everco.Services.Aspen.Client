// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Net;
    using Auth;
    using Internals;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : AppBase<IAutonomousApp>, IAutonomousApp
    {
        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="AutonomousApp"/>
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        private AutonomousApp(CachePolicy cachePolicy)
        {
            CacheStore.Policy = cachePolicy;
        }

        /// <summary>
        /// Obtiene una instancia que permite conectar con el servico Aspen.
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        /// <returns>Instancia de <see cref="IRouting{TFluent}"/> que permite establecer la información de conexión.</returns>
        public static IRouting<IAutonomousApp> Initialize(
            CachePolicy cachePolicy = CachePolicy.CacheIfAvailable)
        {
            return new AutonomousApp(cachePolicy);
        }

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación.
        /// </summary>
        /// <returns>Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.</returns>
        public ISession<IAutonomousApp> Authenticate()
        {
            if (!ServiceLocator.Instance.Runtime.IsTestingExecuting)
            {
                this.AuthToken = CacheStore.Get<AuthToken>(CacheKeys.CurrentAuthToken);
                if (this.AuthToken != null)
                {
                    return this;
                }
            }

            this.InitializeClient();
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.Signin);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSigninPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, null);
            IRestResponse response = base.Execute(request);
            this.AuthToken = JsonConvert.DeserializeObject<AuthToken>(this.DecodeJwtResponse(response.Content));
            CacheStore.Add(CacheKeys.CurrentAuthToken, this.AuthToken);
            return this;
        }

        /// <summary>
        /// Envía una solicitud al servicio para actualizar el secreto de la aplicación.
        /// </summary>
        /// <param name="newSecret">El nuevo secreto para la aplicación.</param>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        public void UpdateApiSecret(string newSecret)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(newSecret, nameof(newSecret));
            }

            this.InitializeClient();
            IRestRequest request = new AspenRequest(
                Scope.Autonomous,
                EndpointMapping.ApiSecret,
                contentType: "application/x-www-form-urlencoded");

            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSigninPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, null);
            request.AddParameter("NewValue", newSecret);
            base.Execute(request);
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
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(
                request,
                this.JwtEncoder,
                this.AppIdentity.ApiSecret,
                this.AuthToken.Token);
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
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(
                request,
                this.JwtEncoder,
                this.AppIdentity.ApiSecret,
                this.AuthToken.Token);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, apiVersion);
            base.Execute(request);
        }
    }
}