// -----------------------------------------------------------------------
// <copyright file="ServiceLocator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 08:31 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client
{
    using System.Net;
    using Internals;
    using JWT;
    using JWT.Serializers;
    using Providers;
    using SimpleInjector;

    /// <summary>
    /// Expone las operaciones que permiten encapsular y sobrescribir las dependencias del cliente.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static readonly object padlock = new object();

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IServiceLocator instance;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private Container container;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServiceLocator" />
        /// </summary>
        private ServiceLocator()
        {
            this.container = new Container();
            this.container.Options.AllowOverridingRegistrations = true;
            this.container.RegisterInstance<INonceGenerator>(new GuidNonceGenerator());
            this.container.RegisterInstance<IEpochGenerator>(new UnixEpochGenerator());
            this.container.RegisterInstance<IRequestHeaderNames>(new DefaultRequestHeaderNames());
            this.container.RegisterInstance<IHeadersManager>(new HeadersManager());
            this.container.RegisterInstance<IJsonSerializer>(new JsonNetSerializer());
            this.container.RegisterInstance<IWebProxy>(new NullWebProxy());
            this.container.Verify();
        }

        /// <summary>
        /// Obtiene la instancia actual de la clase.
        /// </summary>
        /// <value>Referencia de la instancia actual.</value>
        public static IServiceLocator Instance => instance ?? (instance = new ServiceLocator());

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para generar valores Epoch.
        /// </summary>
        public IEpochGenerator EpochGenerator => this.container?.GetInstance<IEpochGenerator>();

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para agregar las cabeceras personalizadas requeridas por el servicio.
        /// </summary>
        public IHeadersManager HeadersManager => this.container?.GetInstance<IHeadersManager>();

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para serializar los valores en el Payload.
        /// </summary>
        public IJsonSerializer JwtJsonSerializer => this.container?.GetInstance<IJsonSerializer>();

        /// <summary>
        /// Obtiene la instancia del proveedor de escritura de información de seguimiento.
        /// </summary>
        public ILoggingProvider LoggingProvider => this.container?.GetInstance<ILoggingProvider>();

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para generar valores Nonce.
        /// </summary>
        public INonceGenerator NonceGenerator => this.container?.GetInstance<INonceGenerator>();

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para obtener los nombres de las cabeceras personalizadas del servicio.
        /// </summary>
        public IRequestHeaderNames RequestHeaderNames => this.container?.GetInstance<IRequestHeaderNames>();

        /// <summary>
        /// Obtiene la instancia del servidor proxy que se debe utilizar para las conexiones con el servicio.
        /// </summary>
        public IWebProxy WebProxy => this.container?.GetInstance<IWebProxy>();

        /// <summary>
        /// Registra una instancia de <see cref="IHeadersManager"/> que permite agregar las cabeceras personalizadas.
        /// </summary>
        /// <param name="headersManager">Instancia que implementa <see cref="IHeadersManager"/>.</param>
        public void RegisterInstanceOfApiSignManager(IHeadersManager headersManager)
        {
            Throw.IfNull(headersManager, nameof(headersManager));
            this.RegisterInstance(headersManager: headersManager);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IEpochGenerator" /> para la generación de valores Epoch.
        /// </summary>
        /// <param name="epochGenerator">Instancia que implementa <see cref="IEpochGenerator" />.</param>
        public void RegisterInstanceOfEpochGenerator(IEpochGenerator epochGenerator)
        {
            Throw.IfNull(epochGenerator, nameof(epochGenerator));
            this.RegisterInstance(epochGenerator: epochGenerator);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IJsonSerializer" /> que permite serializar valores en el Payload.
        /// </summary>
        /// <param name="jwtSerializer">Instancia que implementa <see cref="JWT.IJsonSerializer" />.</param>
        public void RegisterInstanceOfJwtJsonSerializer(IJsonSerializer jwtSerializer)
        {
            Throw.IfNull(jwtSerializer, nameof(jwtSerializer));
            this.RegisterInstance(jwtJsonSerializer: jwtSerializer);
        }

        /// <summary>
        /// Registra una instancia de <see cref="ILoggingProvider" /> para la escritura de trazas de seguimiento del cliente.
        /// </summary>
        /// <param name="loggingProvider">Instancia que implementa <see cref="ILoggingProvider" />.</param>
        public void RegisterInstanceOfLoggingProvider(ILoggingProvider loggingProvider)
        {
            Throw.IfNull(loggingProvider, nameof(loggingProvider));
            this.RegisterInstance(loggingProvider: loggingProvider);
        }

        /// <summary>
        /// Registra una instancia de <see cref="INonceGenerator" /> para la generación de valores Nonce.
        /// </summary>
        /// <param name="nonceGenerator">Instancia que implementa <see cref="INonceGenerator" />.</param>
        public void RegisterInstanceOfNonceGenerator(INonceGenerator nonceGenerator)
        {
            Throw.IfNull(nonceGenerator, nameof(nonceGenerator));
            this.RegisterInstance(nonceGenerator);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IRequestHeaderNames" /> para la generación de valores.
        /// </summary>
        /// <param name="requestHeaderNames">Instancia que implementa <see cref="IRequestHeaderNames" />.</param>
        public void RegisterInstanceOfRequestHeaders(IRequestHeaderNames requestHeaderNames)
        {
            Throw.IfNull(requestHeaderNames, nameof(requestHeaderNames));
            this.RegisterInstance(requestHeaderNames: requestHeaderNames);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IWebProxy"/> que permite establecer el proxy que se utiliza para conectar con el servicio.
        /// </summary>
        /// <param name="webProxy">Instancia que implementa <see cref="IWebProxy"/>.</param>
        public void RegisterInstanceOfWebProxy(IWebProxy webProxy)
        {
            Throw.IfNull(webProxy, nameof(webProxy));
            this.RegisterInstance(webProxy: webProxy);
        }

        /// <summary>
        /// Restablece esta instancia con los valores predeterminados.
        /// </summary>
        public void Reset()
        {
            this.RegisterInstance(
                nonceGenerator: new GuidNonceGenerator(),
                epochGenerator: new UnixEpochGenerator(),
                requestHeaderNames: new DefaultRequestHeaderNames(),
                headersManager: new HeadersManager(),
                jwtJsonSerializer: new JsonNetSerializer(),
                webProxy: new NullWebProxy(),
                loggingProvider: new NullLoggingProvider());
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServiceLocator" />
        /// </summary>
        /// <param name="nonceGenerator">Instancia de <see cref="INonceGenerator"/> que se utiliza para inicializar el proveedor de valores nonce o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="epochGenerator">Instancia de <see cref="IEpochGenerator"/> que se utiliza para inicializar el proveedor de valores epoch o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="headersManager">Instancia de <see cref="IHeadersManager"/> que se utiliza para inicializar el proveedor de cabeceras para las solicitudes al servicio o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="requestHeaderNames">Instancia de <see cref="IRequestHeaderNames"/> que se utiliza para inicializar el proveedor los nombres de cabeceras o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="jwtJsonSerializer">Instancia de <see cref="IJsonSerializer"/> que se utiliza para inicializar el proveedor de serialización y deserialización de JWT o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="webProxy">Instancia de <see cref="IWebProxy"/> que se utiliza para inicializar el proveedor del servidor proxy o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="loggingProvider">Instancia de <see cref="ILoggingProvider"/> que se utiliza para inicializar el proveedor de escritura de trazas de seguimiento o <c>null</c> para utilizar la instancia predeterminada.</param>
        private void RegisterInstance(
            INonceGenerator nonceGenerator = null,
            IEpochGenerator epochGenerator = null,
            IHeadersManager headersManager = null,
            IRequestHeaderNames requestHeaderNames = null,
            IJsonSerializer jwtJsonSerializer = null,
            IWebProxy webProxy = null,
            ILoggingProvider loggingProvider = null)
        {
            lock (padlock)
            {
                INonceGenerator instanceOfNonceGenerator = nonceGenerator ?? this.NonceGenerator ?? new GuidNonceGenerator();
                IEpochGenerator instanceOfEpochGenerator = epochGenerator ?? this.EpochGenerator ?? new UnixEpochGenerator();
                IRequestHeaderNames instanceOfRequestHeaderNames = requestHeaderNames ?? this.RequestHeaderNames ?? new DefaultRequestHeaderNames();
                IHeadersManager instanceOfHeadersManager = headersManager ?? this.HeadersManager ?? new HeadersManager();
                IJsonSerializer instanceOfJwtJsonSerializer = jwtJsonSerializer ?? this.JwtJsonSerializer ?? new JsonNetSerializer();
                IWebProxy instanceOfWebProxy = webProxy ?? this.WebProxy ?? new NullWebProxy();
                ILoggingProvider instanceOfLoggingProvider = loggingProvider ?? this.LoggingProvider ?? new NullLoggingProvider();

                if (this.container != null)
                {
                    this.container.Dispose();
                    this.container = null;
                }

                this.container = new Container();
                this.container.RegisterInstance(instanceOfNonceGenerator);
                this.container.RegisterInstance(instanceOfEpochGenerator);
                this.container.RegisterInstance(instanceOfRequestHeaderNames);
                this.container.RegisterInstance(instanceOfHeadersManager);
                this.container.RegisterInstance(instanceOfJwtJsonSerializer);
                this.container.RegisterInstance(instanceOfWebProxy);
                this.container.RegisterInstance(instanceOfLoggingProvider);
                this.container.Verify();
            }
        }
    }
}