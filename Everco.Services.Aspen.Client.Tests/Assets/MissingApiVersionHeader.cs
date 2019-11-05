// -----------------------------------------------------------------------
// <copyright file="MissingApiVersionHeader.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class MissingApiVersionHeader : MissingHeadersManager
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly Func<string> apiVersionHeaderBehavior = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <remarks>
        /// Con el constructor predeterminado no se establece el comportamiento para los encabezados.
        /// </remarks>
        public MissingApiVersionHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <param name="apiVersionHeaderBehavior">Expresión que determina el comportamiento del valor para la cabecera del ApiKey.</param>
        public MissingApiVersionHeader(Func<string> apiVersionHeaderBehavior)
        {
            this.apiVersionHeaderBehavior = apiVersionHeaderBehavior;
        }

        /// <summary>
        /// Agrega la cabecera que identifica el número de versión del API solicitada.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        public override void AddApiVersionHeader(IRestRequest request, string apiVersion)
        {
            if (this.apiVersionHeaderBehavior == null)
            {
                return;
            }

            string apiVersionHeaderValue = this.apiVersionHeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, apiVersionHeaderValue);
        }
    }
}
