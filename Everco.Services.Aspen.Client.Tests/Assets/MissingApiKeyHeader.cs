// -----------------------------------------------------------------------
// <copyright file="MissingHeadersManager.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using Auth;
    using JWT;
    using Providers;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que establece comportamientos de prueba para las cabeceras personalizadas esperadas por el servicio.
    /// </summary>
    internal class MissingApiKeyHeader : MissingHeadersManager
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly Func<string> apiKeyHeaderBehavior = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <remarks>
        /// Con el constructor predeterminado no se establece el comportamiento para los encabezados.
        /// </remarks>
        public MissingApiKeyHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <param name="apiKeyHeaderBehavior">Expresión que determina el comportamiento del valor para la cabecera del ApiKey.</param>
        public MissingApiKeyHeader(Func<string> apiKeyHeaderBehavior)
        {
            this.apiKeyHeaderBehavior = apiKeyHeaderBehavior;
        }
        
        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public override void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            if (this.apiKeyHeaderBehavior == null)
            {
                return;
            }

            string apiKeyHeaderValue = this.apiKeyHeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKeyHeaderValue);
        }
    }
}
