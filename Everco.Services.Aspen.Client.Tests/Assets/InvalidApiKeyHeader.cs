// -----------------------------------------------------------------------
// <copyright file="InvalidApiKeyHeader.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using Everco.Services.Aspen.Client.Providers;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que establece comportamientos personalizados para fines de pruebas de la cabecera de la versión del API.
    /// </summary>
    internal class InvalidApiKeyHeader : InvalidHeadersManager
    {
        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="InvalidApiKeyHeader" />.
        /// </summary>
        private InvalidApiKeyHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        private InvalidApiKeyHeader(Func<string> headerBehavior) : base(headerBehavior)
        {
        }

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> con un comportamiento de personalizado para el encabezado.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager WithHeaderBehavior(Func<string> headerBehavior) => new InvalidApiKeyHeader(headerBehavior);

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> donde se evita agregar el encabezado personalizado.
        /// </summary>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager AvoidingHeader() => new InvalidApiKeyHeader();

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public override void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            if (this.HeaderBehavior == null)
            {
                return;
            }

            string apiKeyHeaderValue = this.HeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKeyHeaderValue);
        }
    }
}
