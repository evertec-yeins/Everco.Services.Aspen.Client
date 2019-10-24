﻿// -----------------------------------------------------------------------
// <copyright file="AppBase.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System;
    using System.Net;
    using Auth;
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
        /// Obtiene el token de autenticación emitido para la sesión.
        /// </summary>
        public IAuthToken AuthToken { get; internal set; }

        /// <summary>
        /// Obtiene la información de la identidad de la aplicación.
        /// </summary>
        protected IAppIdentity AppIdentity { get; private set; }

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
        /// Decodifica una cadena JWT.
        /// </summary>
        /// <param name="jwt">La cadena a decodificar.</param>
        /// <returns>Json que conforma el contenido de la respuesta.</returns>
        public string DecodeJwtResponse(string jwt)
        {
            return this.JwtDecoder.Decode(jwt, this.AppIdentity.ApiSecret, true);
        }

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
            this.RoutingTo(endpointProvider.BaseUrl, Convert.ToInt32(endpointProvider.Timeout.TotalSeconds));
            return this;
        }

        /// <summary>
        /// Establece la Url base para las solicitudes al servicio Aspen.
        /// </summary>
        /// <param name="baseUrl">La Url base para las solicitudes realizadas hacia al servicio Aspen. Ejemplo: <a>http://localhost/api</a></param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio. Valor predeterminado: 15 segundos.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        public IAppIdentity<TFluent> RoutingTo(string baseUrl, int? timeout = null)
        {
            Throw.IfNullOrEmpty(nameof(baseUrl), baseUrl);
            this.endpoint = new Uri(baseUrl.TrimEnd('/'), UriKind.Absolute);
            int defaultTimeout = 15;
            int waitForSeconds = Math.Max(timeout ?? defaultTimeout, defaultTimeout);
            this.timeout = TimeSpan.FromSeconds(waitForSeconds);
            return this;
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
        /// Inicializa la instancia del tipo <see cref="RestSharp.RestClient"/> que se utilza para enviar las solicitudes al servicio Aspen.
        /// </summary>
        protected void InitializeClient()
        {
            this.RestClient = new RestClient(this.endpoint.ToString().TrimEnd('/'));
            this.JwtEncoder = new JwtEncoder(this.algorithm, ServiceLocator.Instance.JwtJsonSerializer, this.urlEncoder);
            this.validator = new JwtValidator(ServiceLocator.Instance.JwtJsonSerializer, this.datetimeProvider);
            this.JwtDecoder = new JwtDecoder(ServiceLocator.Instance.JwtJsonSerializer, this.validator, this.urlEncoder);
            this.RestClient.Timeout = (int)this.timeout.TotalMilliseconds;

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