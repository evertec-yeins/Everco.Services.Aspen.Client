// -----------------------------------------------------------------------
// <copyright file="InvalidApiVersionHeader.cs" company="Evertec Colombia">
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
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class InvalidApiVersionHeader : InvalidHeadersManager
    {
        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="InvalidApiVersionHeader" />.
        /// </summary>
        private InvalidApiVersionHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        private InvalidApiVersionHeader(Func<string> headerBehavior) : base(headerBehavior)
        {
        }

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> con un comportamiento de personalizado para el encabezado.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager WithHeaderBehavior(Func<string> headerBehavior) => new InvalidApiVersionHeader(headerBehavior);

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> donde se evita agregar el encabezado personalizado.
        /// </summary>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager AvoidingHeader() => new InvalidApiVersionHeader();

        /// <summary>
        /// Agrega la cabecera que identifica el número de versión del API solicitada.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        public override void AddApiVersionHeader(IRestRequest request, string apiVersion)
        {
            if (this.HeaderBehavior == null)
            {
                return;
            }

            string apiVersionHeaderValue = this.HeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, apiVersionHeaderValue);
        }
    }
}
