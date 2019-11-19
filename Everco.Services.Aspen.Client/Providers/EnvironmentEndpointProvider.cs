// -----------------------------------------------------------------------
// <copyright file="EnvironmentEndpointProvider.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Implementa la Url base del servicio Aspen y el tiempo de espera para las respuestas a partir del ambiente.
    /// </summary>
    public class EnvironmentEndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string EnvironmentName = "ASPEN:PUBLIC_SERVICE_URL";

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string NonSet = "NONSET";

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IEndpointProvider localEndpoint;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IEndpointProvider publicEndpoint;

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="EnvironmentEndpointProvider" />.
        /// </summary>
        private EnvironmentEndpointProvider()  
            : this($"{Environment.GetEnvironmentVariable("ASPEN:PUBLIC_SERVICE_URL")}/api", null)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnvironmentEndpointProvider"/>.
        /// </summary>
        /// <param name="baseUrl">La Url base del servicio.</param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio. Valor predeterminado: 15 segundos.</param>
        private EnvironmentEndpointProvider(string baseUrl, int? timeout = null)
        {
            this.BaseUrl = baseUrl;
            int defaultTimeout = 15;
            int waitForSeconds = Math.Max(timeout ?? defaultTimeout, 1);
            this.Timeout = TimeSpan.FromSeconds(waitForSeconds);
        }

        /// <summary>
        /// Obtiene una instancia de <see cref="IEndpointProvider"/> con la Url local del servicio de Aspen.
        /// </summary>
        public static IEndpointProvider Local => localEndpoint ??= new EnvironmentEndpointProvider();

        /// <summary>
        /// Obtiene una instancia de <see cref="IEndpointProvider"/> con la Url Pública del servicio de Aspen almacenada en variable de ambiente.
        /// </summary>
        /// <exception cref="InvalidOperationException">Define an environment variable ASPEN Public Service Url.</exception>
        public static IEndpointProvider Public
        {
            get
            {
                if (publicEndpoint != null)
                {
                    return publicEndpoint;
                }

                string publicServiceUrl = Environment.GetEnvironmentVariable(EnvironmentName).DefaultIfNullOrEmpty(NonSet);
                if (publicServiceUrl.Equals(NonSet, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Define an environment variable with name: '{EnvironmentName}' and set value for ASPEN Public Service Url.");
                }

                string baseUrl = $"{publicServiceUrl.TrimEnd('/')}/api";
                publicEndpoint = new EnvironmentEndpointProvider(baseUrl);
                return publicEndpoint;
            }
        }

        /// <summary>
        /// Obtiene la Url base para las solicitudes realizadas hacia el servicio Aspen.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Obtiene el tiempo de espera para las respuestas del servicio Aspen.
        /// </summary>
        public TimeSpan Timeout { get; }
    }
}