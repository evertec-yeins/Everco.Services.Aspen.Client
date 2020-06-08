// -----------------------------------------------------------------------
// <copyright file="AppBase.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
namespace Everco.Services.Aspen.Client.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Auth;
    using Identity;
    using Internals;
    using JWT;
    using JWT.Algorithms;
    using Newtonsoft.Json;
    using Providers;
    using RestSharp;

    /// <summary>
    /// Clase base para la implementación de clientes de conexión con el servicio Aspen.
    /// </summary>
    /// <typeparam name="TFluent">Tipo concreto de la aplicación que se conecta.</typeparam>
    public abstract class AppBase<TFluent> : IRouting<TFluent>, IAppIdentity<TFluent>, ISession<TFluent>
        where TFluent : class
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly IJwtAlgorithm algorithm = new HMACSHA256Algorithm();

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly IDateTimeProvider datetimeProvider = new UtcDateTimeProvider();

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private Uri endpoint;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private TimeSpan timeout;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private IJwtValidator validator;

        /// <summary>
        /// Obtiene la información de la identidad de la aplicación.
        /// </summary>
        public IAppIdentity AppIdentity { get; internal set; }

        /// <summary>
        /// Obtiene el token de autenticación emitido para la sesión.
        /// </summary>
        public IAuthToken AuthToken { get; internal set; }

        /// <summary>
        /// Para uso interno.
        /// </summary>
        protected IJwtDecoder JwtDecoder { get; private set; }

        /// <summary>
        /// Para uso interno.
        /// </summary>
        protected IJwtEncoder JwtEncoder { get; private set; }

        /// <summary>
        /// Para uso interno.
        /// </summary>
        protected IRestClient RestClient { get; private set; }

        /// <summary>
        /// Envía una solicitud de autenticación al servicio Aspen.
        /// </summary>
        /// <returns>Instancia de <see cref="ISession{TFluent}"/> que se puede utilizar para solicitar más recursos de información al servicio Aspen.</returns>
        TFluent ISession<TFluent>.GetClient()
        {
            this.InitializeClient();
            return this as TFluent;
        }

        /// <summary>
        /// Establece la Url base para las solicitudes al servicio Aspen.
        /// </summary>
        /// <param name="endpointProvider">Instancia con la configuración del servicio Aspen.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        public IAppIdentity<TFluent> RoutingTo(IEndpointProvider endpointProvider)
        {
            Throw.IfNull(endpointProvider, nameof(endpointProvider));
            this.RoutingTo(endpointProvider.Url, Convert.ToInt32(endpointProvider.Timeout.TotalSeconds));
            return this;
        }

        /// <summary>
        /// Establece la Url base para las solicitudes al servicio Aspen.
        /// </summary>
        /// <param name="url">La Url base para las solicitudes realizadas hacia al servicio Aspen. Ejemplo: <a>http://localhost/api</a></param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio. Valor predeterminado: 15 segundos.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        public IAppIdentity<TFluent> RoutingTo(string url, int? timeout = null)
        {
            Throw.IfNullOrEmpty(url, nameof(url));
            this.endpoint = new Uri(url.TrimEnd('/'), UriKind.Absolute);
            const int DefaultTimeout = 15;
            int waitForSeconds = Math.Max(timeout ?? DefaultTimeout, DefaultTimeout);
            this.timeout = TimeSpan.FromSeconds(waitForSeconds);
            return this;
        }

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="url">La URL para las solicitudes hacia al API de ASPEN realizadas por esta instancia de cliente. Ejemplo: <a>http://localhost/api</a>.</param>
        /// <param name="timeout">El tiempo de espera para las respuesta de las solicitudes al servicio o <c>null</c> para establecer el valor predeterminado (15 segundos)</param>
        /// <returns>
        /// Instancia de <see cref="IAppIdentity" /> que permite establecer los datos de conexión con el servicio.
        /// </returns>
        public IAppIdentity<TFluent> RoutingTo(Uri url, TimeSpan? timeout = null)
        {
            Throw.IfNull(nameof(url), nameof(url));
            int? waitForSeconds = Convert.ToInt32(timeout?.TotalSeconds);
            return this.RoutingTo(url.ToString(), waitForSeconds);
        }

        /// <summary>
        /// Establece la URL y la identidad para las solicitudes al servicio Aspen a partir de los valores predeterminados.
        /// </summary>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        public TFluent WithDefaults()
        {
            this.RoutingTo(ServiceLocator.Instance.DefaultEndpoint);
            this.WithIdentity(ServiceLocator.Instance.DefaultIdentity);
            return this as TFluent;
        }

        /// <summary>
        /// Establece la identidad de la aplicación solicitante.
        /// </summary>
        /// <param name="apiKey">El ApiKey asignado a la aplicación que se está conectando con el servicio Aspen.</param>
        /// <param name="apiSecret">El ApiSecret asignado a la aplicación que se está conectando con el servicio Aspen.</param>
        /// <returns>Instancia de <see cref="IAutonomousApp" /> que permite establecer conexión con el servicio Aspen.</returns>
        public TFluent WithIdentity(string apiKey, string apiSecret)
        {
            Throw.IfNullOrEmpty(nameof(apiKey), apiKey);
            Throw.IfNullOrEmpty(nameof(apiSecret), apiSecret);
            this.AppIdentity = new StaticAppIdentity(apiKey, apiSecret);
            return this as TFluent;
        }

        /// <summary>
        /// Establece la identidad de la aplicación solicitante.
        /// </summary>
        /// <param name="appIdentity">Identidad de la aplicación solicitante.</param>
        /// <returns>Instancia de <see cref="T:Everco.Services.Aspen.Client.Fluent.IAutonomousApp" /> que permite conectar con el servicio Aspen.</returns>
        public TFluent WithIdentity(IAppIdentity appIdentity)
        {
            Throw.IfNull(appIdentity, nameof(appIdentity));
            this.AppIdentity = appIdentity;
            return this as TFluent;
        }

        /// <summary>
        /// Decodifica una cadena JWT.
        /// </summary>
        /// <param name="jwt">La cadena a decodificar.</param>
        /// <returns>Json que conforma el contenido de la respuesta.</returns>
        protected string DecodeJwtResponse(string jwt)
        {
            return this.JwtDecoder.Decode(jwt, this.AppIdentity.ApiSecret, true);
        }

        /// <summary>
        /// Envía la solicitud al servicio Aspen.
        /// </summary>
        /// <param name="request">Información de la solicitud.</param>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        /// <returns>Instancia de <see cref="IRestResponse"/> que contiene los datos de la respuesta generada por la solicitud al API.</returns>
        protected IRestResponse Execute(IRestRequest request)
        {
            const string NonSet = "NONSET";
            ServiceLocator.Instance.LoggingProvider.WriteDebug("========== Request Start ==========");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Uri => {this.RestClient.BaseUrl}{request.Resource}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Method => {request.Method}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Proxy => {(ServiceLocator.Instance.WebProxy as WebProxy)?.Address?.ToString() ?? NonSet}");
#if DEBUG
            Dictionary<string, object> requestBody = request.Parameters.GetBody();
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"RequestBody => {JsonConvert.SerializeObject(requestBody)}");
            Dictionary<string, object> requestHeaders = request.Parameters.GetHeaders();
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"RequestHeaders => {JsonConvert.SerializeObject(requestHeaders)}");

            string payload = requestHeaders.GetValueOrDefault(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName) as string ?? NonSet;
            try
            {
                payload = this.JwtDecoder.Decode(payload);
                ServiceLocator.Instance.LoggingProvider.WriteDebug($"Payload => {JsonConvert.DeserializeObject(payload)}");
            }
            catch (Exception)
            {
                ServiceLocator.Instance.LoggingProvider.WriteDebug($"Payload => {payload}");
            }
#endif
            IRestResponse response = this.RestClient.Execute(request);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatusCode => {(int)response.StatusCode} ({response.StatusCode})");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatusDescription => {response.StatusDescription}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatus => {response.ResponseStatus}");
            string responseContentType = response.ContentType.DefaultIfNullOrEmpty(NonSet);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContentType => {responseContentType}");
            string responseContent = responseContentType.Contains("text/html")
                ? "[TEXT/HTML]"
                : response.Content.DefaultIfNullOrEmpty(NonSet);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContent => {responseContent}");
#if DEBUG
            Dictionary<string, object> responseHeaders = response.Headers.GetHeaders();
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseHeaders => {JsonConvert.SerializeObject(responseHeaders)}");
#endif
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"DumpLink => {response.GetHeader("X-PRO-Response-Dump").DefaultIfNullOrEmpty(NonSet)}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug("========== Request End ==========");

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response;
        }

        /// <summary>
        /// Inicializa la instancia del tipo <see cref="RestSharp.RestClient"/> que se utilza para enviar las solicitudes al servicio Aspen.
        /// </summary>
        protected void InitializeClient()
        {
            this.JwtEncoder = new JwtEncoder(this.algorithm, ServiceLocator.Instance.JwtJsonSerializer, this.urlEncoder);
            this.validator = new JwtValidator(ServiceLocator.Instance.JwtJsonSerializer, this.datetimeProvider);
            this.JwtDecoder = new JwtDecoder(ServiceLocator.Instance.JwtJsonSerializer, this.validator, this.urlEncoder, this.algorithm);
            this.RestClient = new RestClient(this.endpoint.ToString().TrimEnd('/'))
            {
                Timeout = (int)this.timeout.TotalMilliseconds
            };
            this.RestClient.UseSerializer(() => JsonNetSerializer.Default);

            IWebProxy webProxy = ServiceLocator.Instance.WebProxy;
            if (webProxy.GetType() != typeof(NullWebProxy))
            {
                this.RestClient.Proxy = ServiceLocator.Instance.WebProxy;
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }
    }
}