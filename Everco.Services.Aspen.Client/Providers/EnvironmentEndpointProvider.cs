// -----------------------------------------------------------------------
// <copyright file="EnvironmentEndpointProvider.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Implementa la Url base del servicio Aspen y el tiempo de espera para las respuestas a partir del ambiente.
    /// </summary>
    public class EnvironmentEndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Para uso interno;
        /// </summary>
        private const string NonSet = "NONSET";

        /// <summary>
        /// Para uso interno;
        /// </summary>
        private static IEndpointProvider instance;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnvironmentEndpointProvider"/>
        /// </summary>
        /// <param name="url">La Url base del servicio.</param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio. Valor predeterminado: 15 segundos.</param>
        public EnvironmentEndpointProvider(string url, int timeout = 15)
        {
            this.Url = $"{url.TrimEnd('/')}/api";
            int waitForSeconds = Math.Max(timeout, 10);
            this.Timeout = TimeSpan.FromSeconds(waitForSeconds);
        }

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="EnvironmentEndpointProvider" />.
        /// </summary>
        private EnvironmentEndpointProvider()
        {
            string serviceUrl = Environment.GetEnvironmentVariable("ASPEN:PUBLIC_SERVICE_URL").DefaultIfNullOrEmpty(NonSet);
            if (serviceUrl.Equals(NonSet, StringComparison.OrdinalIgnoreCase))
            {
                serviceUrl = "https://dev.evertecinc.co:8082";
            }

            this.Url = $"{serviceUrl.TrimEnd('/')}/api";

            int serviceTimeout = Environment.GetEnvironmentVariable("ASPEN:PUBLIC_SERVICE_TIMEOUT")
                .DefaultIfNullOrEmpty(NonSet)
                .Convert(15);
            int waitForSeconds = Math.Max(serviceTimeout, 1);
            this.Timeout = TimeSpan.FromSeconds(waitForSeconds);
        }

        /// <summary>
        /// Obtiene una instancia de <see cref="IEndpointProvider"/> con la URL del servicio de ASPEN.
        /// </summary>
        public static IEndpointProvider Default => instance ?? (instance = new EnvironmentEndpointProvider());

        /// <summary>
        /// Obtiene el tiempo de espera para las respuestas del servicio Aspen.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Obtiene la Url base para las solicitudes realizadas hacia el servicio Aspen.
        /// </summary>
        public string Url { get; }
    }
}