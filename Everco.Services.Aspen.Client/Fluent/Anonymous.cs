// -----------------------------------------------------------------------
// <copyright file="Anonymous.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Everco.Services.Aspen.Client.Internals;
    using Everco.Services.Aspen.Client.Providers;
    using Identity;
    using LazyCache;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// Expone las operaciones que permite conectar con las operaciones disponibles en el servicio Aspen que no requieren firma.
    /// </summary>
    public sealed partial class Anonymous : IAnonymous
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly IAppCache cache;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly MemoryCacheEntryOptions cacheOptions;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private Uri endpoint;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private IRestClient restClient;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private TimeSpan timeout;

        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="Fluent.Anonymous"/>
        /// </summary>
        private Anonymous()
        {
            this.cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };

            this.cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                ServiceLocator.Instance.LoggingProvider.WriteDebug($"Anonymous => Entry: '{key}' is evicted from cache. Season: {reason}");
            });

            this.cache = new CachingService();
        }

        /// <summary>
        /// Obtiene una instancia que permite conectar con el servico Aspen.
        /// </summary>
        /// <returns>Instancia de <see cref="IRouting{TFluent}"/> que permite establecer la información de conexión.</returns>
        public static IRouting Initialize() => new Anonymous();

        /// <summary>
        /// Obtiene la instancia actual configurada para interactuar con el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Instancia de <seealso cref="IAnonymous"/> que permite interactuar con el servicio Aspen.
        /// </returns>
        public IAnonymous GetClient()
        {
            this.InitializeClient();
            return this;
        }

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="endpointProvider">Instancia con la configuración del servicio ASPEN.</param>
        /// <returns>
        /// Instancia de <see cref="ISession" /> que permite acceder a los datos de conexión con el servicio.
        /// </returns>
        public ISession RoutingTo(IEndpointProvider endpointProvider)
        {
            Throw.IfNull(endpointProvider, nameof(endpointProvider));
            this.RoutingTo(new Uri(endpointProvider.Url), endpointProvider.Timeout);
            return this;
        }

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="url">La URL para las solicitudes realizadas hacia al servicio ASPEN. Ejemplo: <a>http://localhost/api</a></param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio o <c>null</c> para establecer el valor predeterminado (15 segundos)</param>
        /// <returns>
        /// Instancia de <see cref="ISession" /> que permite acceder a los datos de conexión con el servicio.
        /// </returns>
        public ISession RoutingTo(string url, int? timeout = null)
        {
            Throw.IfNullOrEmpty(url, nameof(url));
            this.endpoint = new Uri(url.TrimEnd('/'), UriKind.Absolute);
            int defaultTimeout = 15;
            int waitForSeconds = Math.Max(timeout ?? defaultTimeout, defaultTimeout);
            this.timeout = TimeSpan.FromSeconds(waitForSeconds);
            return this;
        }

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="url">La URL para las solicitudes hacia al API de ASPEN realizadas por esta instancia de cliente. Ejemplo: <a>http://localhost/api</a>.</param>
        /// <param name="timeout">El tiempo de espera para las respuesta de las solicitudes al servicio o <c>null</c> para establecer el valor predeterminado (15 segundos)</param>
        /// <returns>
        /// Instancia de <see cref="ISession" /> que permite acceder a los datos de conexión con el servicio.
        /// </returns>
        public ISession RoutingTo(Uri url, TimeSpan? timeout = null)
        {
            Throw.IfNull(nameof(url), nameof(url));
            int? waitForSeconds = Convert.ToInt32(timeout?.TotalSeconds);
            return this.RoutingTo(url.ToString(), waitForSeconds);
        }

        /// <summary>
        /// Establece la URL para las solicitudes al servicio Aspen a partir de los valores predeterminados.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="ISession" /> que permite acceder a los datos de conexión con el servicio.
        /// </returns>
        public ISession WithDefaults()
        {
            this.RoutingTo(ServiceLocator.Instance.DefaultEndpoint);
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
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Resource => {this.restClient.BaseUrl}{request.Resource}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Method => {request.Method}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Proxy => {(ServiceLocator.Instance.WebProxy as WebProxy)?.Address?.ToString() ?? "NONSET"}");
#if DEBUG
            Dictionary<string, object> headers = request.Parameters.GetHeaders();
            Dictionary<string, object> body = request.Parameters.GetBody();
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Headers => {JsonConvert.SerializeObject(headers)}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Body => {JsonConvert.SerializeObject(body)}");
#endif
            IRestResponse response = this.restClient.Execute(request);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusCode => {(int)response.StatusCode} ({response.StatusCode.ToString()})");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusDescription => {response.StatusDescription}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatus => {response.ResponseStatus}");
            string responseContentType = response.ContentType.DefaultIfNullOrEmpty("NONSET");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContentType => {responseContentType}");
            string responseContent = responseContentType.Contains("text/html")
                                         ? "[TEXT/HTML]"
                                         : response.Content.DefaultIfNullOrEmpty("NONSET");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContent => {responseContent}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseDumpLink => {response.GetHeader("X-PRO-Response-Dump").DefaultIfNullOrEmpty("NONSET")}");

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response.StatusCode == HttpStatusCode.NoContent
                ? default
                : JsonConvert.DeserializeObject<TResponse>(response.Content);
        }

        /// <summary>
        /// Envía la solicitud al servicio ASPEN.
        /// </summary>
        /// <param name="request">La información de la solicitud.</param>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        private void Execute(IRestRequest request)
        {
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Resource => {this.restClient.BaseUrl}{request.Resource}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Method => {request.Method}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Proxy => {(ServiceLocator.Instance.WebProxy as WebProxy)?.Address?.ToString() ?? "NONSET"}");
#if DEBUG
            Dictionary<string, object> headers = request.Parameters.GetHeaders();
            Dictionary<string, object> body = request.Parameters.GetBody();
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Headers => {JsonConvert.SerializeObject(headers)}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Body => {JsonConvert.SerializeObject(body)}");
#endif
            IRestResponse response = this.restClient.Execute(request);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusCode => {(int)response.StatusCode} ({response.StatusCode.ToString()})");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusDescription => {response.StatusDescription}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatus => {response.ResponseStatus}");
            string responseContentType = response.ContentType.DefaultIfNullOrEmpty("NONSET");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContentType => {responseContentType}");
            string responseContent = responseContentType.Contains("text/html")
                                         ? "[TEXT/HTML]"
                                         : response.Content.DefaultIfNullOrEmpty("NONSET");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContent => {responseContent}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseDumpLink => {response.GetHeader("X-PRO-Response-Dump").DefaultIfNullOrEmpty("NONSET")}");

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }
        }

        /// <summary>
        /// Inicializa la instancia del tipo <see cref="RestSharp.RestClient"/> que se utilza para enviar las solicitudes al servicio Aspen.
        /// </summary>
        private void InitializeClient()
        {
            this.restClient = new RestClient(this.endpoint.ToString().TrimEnd('/'))
            {
                Timeout = (int)this.timeout.TotalMilliseconds
            };
            this.restClient.UseSerializer(() => JsonNetSerializer.Default);

            IWebProxy webProxy = ServiceLocator.Instance.WebProxy;
            if (webProxy.GetType() != typeof(NullWebProxy))
            {
                this.restClient.Proxy = ServiceLocator.Instance.WebProxy;
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }
    }
}