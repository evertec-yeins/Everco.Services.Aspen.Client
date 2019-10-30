// -----------------------------------------------------------------------
// <copyright file="IServiceLocator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-26 10:17 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client
{
    using System.Net;
    using JWT;
    using Providers;

    /// <summary>
    /// Define las operaciones que permiten encapsular y sobrescribir las dependencias del cliente.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Obtiene la instancia del generador de valores epoch (número de segundos que han transcurrido desde 1970-01-01T00:00:00Z)
        /// </summary>
        IEpochGenerator EpochGenerator { get; }

        /// <summary>
        /// Obtiene la instancia del componente que se utiliza para agregar las cabeceras requeridas por el servicio.
        /// </summary>
        IHeadersManager HeadersManager { get; }

        /// <summary>
        /// Obtiene la instancia del proveedor que se utiliza para serializar o deserializar como JWT, los valores en la carga útil.
        /// </summary>
        IJsonSerializer JwtJsonSerializer { get; }

        /// <summary>
        /// Obtiene la instancia del proveedor de escritura de información de seguimiento.
        /// </summary>
        ILoggingProvider LoggingProvider { get; }

        /// <summary>
        /// Obtiene la instancia del generador de valores nonce (números o cadenas aleatorios para un único uso)
        /// </summary>
        INonceGenerator NonceGenerator { get; }

        /// <summary>
        /// Obtiene la instancia del servicio que se utiliza para obtener los nombres de las cabeceras personalizadas del servicio Aspen.
        /// </summary>
        IHeaderElement RequestHeaderNames { get; }

        /// <summary>
        /// Obtiene una referencia que permite acceder al entorno de ejecución.
        /// </summary>
        IEnvironmentRuntime Runtime { get; }

        /// <summary>
        /// Obtiene la instancia del proxy para la conexión con el servicio de Aspen.
        /// </summary>
        IWebProxy WebProxy { get; }

        /// <summary>
        /// Registra una instancia de <see cref="IEpochGenerator"/> para la generación de valores Epoch.
        /// </summary>
        /// <param name="epochGenerator">Instancia que implementa <see cref="IEpochGenerator"/>.</param>
        void RegisterEpochGenerator(IEpochGenerator epochGenerator);

        /// <summary>
        /// Registra una instancia de <see cref="IHeadersManager"/> que permite agregar las cabeceras personalizadas.
        /// </summary>
        /// <param name="headersManager">Instancia que implementa <see cref="IHeadersManager"/>.</param>
        void RegisterHeadersManager(IHeadersManager headersManager);

        /// <summary>
        /// Registra una instancia de <see cref="IJsonSerializer"/> que permite serializar valores en el Payload.
        /// </summary>
        /// <param name="jwtSerializer">Instancia que implementa <see cref="JWT.IJsonSerializer"/>.</param>
        void RegisterJwtJsonSerializer(IJsonSerializer jwtSerializer);

        /// <summary>
        /// Registra una instancia de <see cref="ILoggingProvider"/> para la escritura de trazas de seguimiento del cliente.
        /// </summary>
        /// <param name="loggingProvider">Instancia que implementa <see cref="ILoggingProvider"/>.</param>
        void RegisterLoggingProvider(ILoggingProvider loggingProvider);

        /// <summary>
        /// Registra una instancia de <see cref="INonceGenerator"/> para la generación de valores Nonce.
        /// </summary>
        /// <param name="nonceGenerator">Instancia que implementa <see cref="INonceGenerator"/>.</param>
        void RegisterNonceGenerator(INonceGenerator nonceGenerator);

        /// <summary>
        /// Registra una instancia de <see cref="IHeaderElement"/> para la generación de valores.
        /// </summary>
        /// <param name="requestHeaderNames">Instancia que implementa <see cref="IHeaderElement"/>.</param>
        void RegisterRequestHeaderNames(IHeaderElement requestHeaderNames);

        /// <summary>
        /// Registra una instancia de <see cref="IWebProxy"/> que permite establecer el proxy que se utiliza para conectar con el servicio.
        /// </summary>
        /// <param name="webProxy">Instancia que implementa <see cref="IWebProxy"/>.</param>
        void RegisterWebProxy(IWebProxy webProxy);

        /// <summary>
        /// Restablece esta instancia con los valores predeterminados.
        /// </summary>
        void Reset();
    }
}