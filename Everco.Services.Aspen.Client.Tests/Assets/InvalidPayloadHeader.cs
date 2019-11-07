// -----------------------------------------------------------------------
// <copyright file="InvalidPayloadHeader.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using Auth;
    using Everco.Services.Aspen.Client.Providers;
    using JWT;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class InvalidPayloadHeader : InvalidHeadersManager
    {
        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="InvalidPayloadHeader" />.
        /// </summary>
        private InvalidPayloadHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        private InvalidPayloadHeader(Func<string> headerBehavior) : base(headerBehavior)
        {
        }

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> con un comportamiento de personalizado para el encabezado.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager WithHeaderBehavior(Func<string> headerBehavior) => new InvalidPayloadHeader(headerBehavior);

        /// <summary>
        /// Crea una instancia de <see cref="IHeadersManager"/> donde se evita agregar el encabezado personalizado.
        /// </summary>
        /// <returns>Instancia de <see cref="IHeadersManager"/> con el comportamiento de personalizado.</returns>
        public static IHeadersManager AvoidingHeader() => new InvalidPayloadHeader();

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public override void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret)
        {
           this.AddPayloadHeader(request);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="userIdentity">La información que se utiliza para autenticar la solicitud en función de un usuario.</param>
        public override void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, IUserIdentity userIdentity)
        {
            this.AddPayloadHeader(request);
        }

        /// <summary>
        ///  Agrega la cabecera con los datos de la carga útil a la solicitud.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        private void AddPayloadHeader(IRestRequest request)
        {
            if (this.HeaderBehavior == null)
            {
                return;
            }

            string payloadHeaderValue = this.HeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, payloadHeaderValue);
        }
    }
}