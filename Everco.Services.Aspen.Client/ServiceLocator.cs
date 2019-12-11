// -----------------------------------------------------------------------
// <copyright file="ServiceLocator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 08:31 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client
{
    using System.Net;
    using Identity;
    using Internals;
    using JWT;
    using Providers;
    using SimpleInjector;
    using JsonNetSerializer = JWT.Serializers.JsonNetSerializer;

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
            EnvironmentRuntime environmentRuntime = new EnvironmentRuntime();
            this.container.RegisterInstance<IEnvironmentRuntime>(environmentRuntime);
            this.container.RegisterInstance<INonceGenerator>(new GuidNonceGenerator());
            this.container.RegisterInstance<IEpochGenerator>(new UnixEpochGenerator());
            this.container.RegisterInstance<IHeaderElement>(new DefaultHeaderElement());
            this.container.RegisterInstance<IHeadersManager>(new DefaultHeadersManager());
            this.container.RegisterInstance<IPayloadClaimElement>(new DefaultPayloadClaimElement());
            this.container.RegisterInstance<IPayloadClaimsManager>(new DefaultPayloadClaimsManager());
            this.container.RegisterInstance<IJsonSerializer>(new JsonNetSerializer());
            this.container.RegisterInstance<IWebProxy>(new NullWebProxy());
            this.container.RegisterInstance<ILoggingProvider>(new NullLoggingProvider());
            this.container.RegisterInstance<IEndpointProvider>(new EnvironmentEndpoint());
            this.container.RegisterInstance<IAppIdentity>(new EnvironmentIdentity());

            if (environmentRuntime.IsDevelopment)
            {
                this.container.RegisterInstance<ILoggingProvider>(new ConsoleLoggingProvider());
            }
        }

        /// <summary>
        /// Obtiene la instancia actual de la clase.
        /// </summary>
        /// <value>Referencia de la instancia actual.</value>
        public static IServiceLocator Instance => instance ?? (instance = new ServiceLocator());

        /// <summary>
        /// Obtiene la instancia del componente predeterminado que se utiliza para obtener la Url del servicio Aspen.
        /// </summary>
        public IEndpointProvider DefaultEndpoint => this.container?.GetInstance<IEndpointProvider>();

        /// <summary>
        /// Obtiene la instancia del componente predeterminado que se utiliza para obtener las credenciales de conexión con el servicio Aspen.
        /// </summary>
        public IAppIdentity DefaultIdentity => this.container?.GetInstance<IAppIdentity>();

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
        /// Obtiene la instancia del servicio que se utiliza para obtener los nombres que se utilizan para las reclamaciones usadas en la carga útil de una solicitud al servicio Aspen.
        /// </summary>
        public IPayloadClaimElement PayloadClaimNames => this.container?.GetInstance<IPayloadClaimElement>();

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para obtener los nombres de las cabeceras personalizadas del servicio.
        /// </summary>
        public IHeaderElement RequestHeaderNames => this.container?.GetInstance<IHeaderElement>();

        /// <summary>
        /// Obtiene la instancia del componente que se utiliza para agregar las reclamaciones en la carga útil requeridas por el servicio.
        /// </summary>
        public IPayloadClaimsManager PayloadClaimsManager => this.container?.GetInstance<IPayloadClaimsManager>();

        /// <summary>
        /// Obtiene una referencia que permite acceder al entorno de ejecución.
        /// </summary>
        public IEnvironmentRuntime Runtime => this.container?.GetInstance<IEnvironmentRuntime>();

        /// <summary>
        /// Obtiene la instancia del servidor proxy que se debe utilizar para las conexiones con el servicio.
        /// </summary>
        public IWebProxy WebProxy => this.container?.GetInstance<IWebProxy>();

        /// <summary>
        /// Registra una instancia de <see cref="IEndpointProvider"/> para la obtención de valores de configuración.
        /// </summary>
        /// <param name="endpointProvider">Instancia que implementa <see cref="IEndpointProvider"/>.</param>
        public void SetDefaultEndpoint(IEndpointProvider endpointProvider)
        {
            Throw.IfNull(endpointProvider, nameof(endpointProvider));
            this.RegisterInstance(endpointProvider: endpointProvider);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IAppIdentity"/> para la obtención de valores de configuración.
        /// </summary>
        /// <param name="appIdentity">Instancia que implementa <see cref="IAppIdentity"/>.</param>
        public void SetDefaultIdentity(IAppIdentity appIdentity)
        {
            Throw.IfNull(appIdentity, nameof(appIdentity));
            this.RegisterInstance(appIdentity: appIdentity);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IEpochGenerator" /> para la generación de valores Epoch.
        /// </summary>
        /// <param name="epochGenerator">Instancia que implementa <see cref="IEpochGenerator" />.</param>
        public void RegisterEpochGenerator(IEpochGenerator epochGenerator)
        {
            Throw.IfNull(epochGenerator, nameof(epochGenerator));
            this.RegisterInstance(epochGenerator: epochGenerator);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IHeadersManager"/> que permite agregar las cabeceras personalizadas.
        /// </summary>
        /// <param name="headersManager">Instancia que implementa <see cref="IHeadersManager"/>.</param>
        public void RegisterHeadersManager(IHeadersManager headersManager)
        {
            Throw.IfNull(headersManager, nameof(headersManager));
            this.RegisterInstance(headersManager: headersManager);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IJsonSerializer" /> que permite serializar valores en el Payload.
        /// </summary>
        /// <param name="jwtSerializer">Instancia que implementa <see cref="JWT.IJsonSerializer" />.</param>
        public void RegisterJwtJsonSerializer(IJsonSerializer jwtSerializer)
        {
            Throw.IfNull(jwtSerializer, nameof(jwtSerializer));
            this.RegisterInstance(jwtJsonSerializer: jwtSerializer);
        }

        /// <summary>
        /// Registra una instancia de <see cref="ILoggingProvider" /> para la escritura de trazas de seguimiento del cliente.
        /// </summary>
        /// <param name="loggingProvider">Instancia que implementa <see cref="ILoggingProvider" />.</param>
        public void RegisterLoggingProvider(ILoggingProvider loggingProvider)
        {
            Throw.IfNull(loggingProvider, nameof(loggingProvider));
            this.RegisterInstance(loggingProvider: loggingProvider);
        }

        /// <summary>
        /// Registra una instancia de <see cref="INonceGenerator" /> para la generación de valores Nonce.
        /// </summary>
        /// <param name="nonceGenerator">Instancia que implementa <see cref="INonceGenerator" />.</param>
        public void RegisterNonceGenerator(INonceGenerator nonceGenerator)
        {
            Throw.IfNull(nonceGenerator, nameof(nonceGenerator));
            this.RegisterInstance(nonceGenerator);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IPayloadClaimElement" /> que define los nombres que se utilizan para las reclamaciones usadas en la carga útil de una solicitud al servicio Aspen.
        /// </summary>
        /// <param name="payloadClaimNames">Instancia que implementa <see cref="IHeaderElement" />.</param>
        public void RegisterPayloadClaimNames(IPayloadClaimElement payloadClaimNames)
        {
            Throw.IfNull(payloadClaimNames, nameof(payloadClaimNames));
            this.RegisterInstance(payloadClaimNames: payloadClaimNames);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IPayloadClaimsManager" /> que permite agregar las reclamaciones requeridas a la carga útil de la solicitud.
        /// </summary>
        /// <param name="payloadClaimsManager">Instancia que implementa <see cref="IPayloadClaimsManager" />.</param>
        public void RegisterPayloadClaimsManager(IPayloadClaimsManager payloadClaimsManager)
        {
            Throw.IfNull(payloadClaimsManager, nameof(payloadClaimsManager));
            this.RegisterInstance(payloadClaimsManager: payloadClaimsManager);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IHeaderElement" /> para la generación de valores.
        /// </summary>
        /// <param name="requestHeaderNames">Instancia que implementa <see cref="IHeaderElement" />.</param>
        public void RegisterRequestHeaderNames(IHeaderElement requestHeaderNames)
        {
            Throw.IfNull(requestHeaderNames, nameof(requestHeaderNames));
            this.RegisterInstance(requestHeaderNames: requestHeaderNames);
        }

        /// <summary>
        /// Registra una instancia de <see cref="IWebProxy"/> que permite establecer el proxy que se utiliza para conectar con el servicio.
        /// </summary>
        /// <param name="webProxy">Instancia que implementa <see cref="IWebProxy"/>.</param>
        public void RegisterWebProxy(IWebProxy webProxy)
        {
            Throw.IfNull(webProxy, nameof(webProxy));
            this.RegisterInstance(webProxy: webProxy);
        }

        /// <summary>
        /// Restablece esta instancia a los valores predeterminados.
        /// </summary>
        public void Reset()
        {
            this.container?.Dispose();
            this.container = null;
            instance = null;
            instance = new ServiceLocator();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServiceLocator" />
        /// </summary>
        /// <param name="nonceGenerator">Instancia de <see cref="INonceGenerator" /> que se utiliza para inicializar el proveedor de valores nonce o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="epochGenerator">Instancia de <see cref="IEpochGenerator" /> que se utiliza para inicializar el proveedor de valores epoch o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="headersManager">Instancia de <see cref="IHeadersManager" /> que se utiliza para inicializar el proveedor de cabeceras para las solicitudes al servicio o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="requestHeaderNames">Instancia de <see cref="IHeaderElement" /> que se utiliza para inicializar el proveedor de los nombres de cabeceras personalizadas o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="payloadClaimNames">Instancia de <see cref="IPayloadClaimElement" /> que se utiliza para inicializar el proveedor de los nombres para las reclamaciones usadas en la carga útil del servicio o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="payloadClaimsManager">Instancia de <see cref="IPayloadClaimsManager" /> que se utiliza para inicializar el proveedor de reclamaciones de la carga útil o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="jwtJsonSerializer">Instancia de <see cref="IJsonSerializer" /> que se utiliza para inicializar el proveedor de serialización y deserialización de JWT o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="webProxy">Instancia de <see cref="IWebProxy" /> que se utiliza para inicializar el proveedor del servidor proxy o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="loggingProvider">Instancia de <see cref="ILoggingProvider" /> que se utiliza para inicializar el proveedor de escritura de trazas de seguimiento o <c>null</c> para utilizar la instancia predeterminada.</param>
        /// <param name="endpointProvider">Instancia que implementa <see cref="IEndpointProvider"/> para la obtención de valores de configuración.</param>
        /// <param name="appIdentity">Instancia que implementa <see cref="IAppIdentity"/> para la obtención de valores de configuración.</param>
        private void RegisterInstance(
            INonceGenerator nonceGenerator = null,
            IEpochGenerator epochGenerator = null,
            IHeadersManager headersManager = null,
            IHeaderElement requestHeaderNames = null,
            IPayloadClaimElement payloadClaimNames = null,
            IPayloadClaimsManager payloadClaimsManager = null,
            IJsonSerializer jwtJsonSerializer = null,
            IWebProxy webProxy = null,
            ILoggingProvider loggingProvider = null,
            IEndpointProvider endpointProvider = null,
            IAppIdentity appIdentity = null)
        {
            lock (padlock)
            {
                INonceGenerator instanceOfNonceGenerator = nonceGenerator ?? this.NonceGenerator ?? new GuidNonceGenerator();
                IEpochGenerator instanceOfEpochGenerator = epochGenerator ?? this.EpochGenerator ?? new UnixEpochGenerator();
                IHeaderElement instanceOfRequestHeaderNames = requestHeaderNames ?? this.RequestHeaderNames ?? new DefaultHeaderElement();
                IPayloadClaimElement instanceOfPayloadClaimNames = payloadClaimNames ?? this.PayloadClaimNames ?? new DefaultPayloadClaimElement();
                IPayloadClaimsManager instanceOfPayloadClaimsManager = payloadClaimsManager ?? this.PayloadClaimsManager ?? new DefaultPayloadClaimsManager();
                IHeadersManager instanceOfHeadersManager = headersManager ?? this.HeadersManager ?? new DefaultHeadersManager();
                IJsonSerializer instanceOfJwtJsonSerializer = jwtJsonSerializer ?? this.JwtJsonSerializer ?? new JsonNetSerializer();
                IWebProxy instanceOfWebProxy = webProxy ?? this.WebProxy ?? new NullWebProxy();
                ILoggingProvider instanceOfLoggingProvider = loggingProvider ?? this.LoggingProvider ?? new NullLoggingProvider();
                IEndpointProvider instanceOfEndpointProvider = endpointProvider ?? new EnvironmentEndpoint();
                IAppIdentity instanceOfAppIdentity = appIdentity ?? new EnvironmentIdentity();

                if (this.container != null)
                {
                    this.container.Dispose();
                    this.container = null;
                }

                this.container = new Container();
                this.container.RegisterInstance<IEnvironmentRuntime>(new EnvironmentRuntime());
                this.container.RegisterInstance(instanceOfNonceGenerator);
                this.container.RegisterInstance(instanceOfEpochGenerator);
                this.container.RegisterInstance(instanceOfRequestHeaderNames);
                this.container.RegisterInstance(instanceOfHeadersManager);
                this.container.RegisterInstance(instanceOfPayloadClaimNames);
                this.container.RegisterInstance(instanceOfPayloadClaimsManager);
                this.container.RegisterInstance(instanceOfJwtJsonSerializer);
                this.container.RegisterInstance(instanceOfWebProxy);
                this.container.RegisterInstance(instanceOfLoggingProvider);
                this.container.RegisterInstance(instanceOfEndpointProvider);
                this.container.RegisterInstance(instanceOfAppIdentity);
            }
        }
    }
}