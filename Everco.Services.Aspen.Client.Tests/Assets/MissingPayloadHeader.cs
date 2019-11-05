// -----------------------------------------------------------------------
// <copyright file="MissingPayloadHeader.cs" company="Evertec Colombia">
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
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class MissingPayloadHeader : MissingHeadersManager
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly Func<string> payloadHeaderBehavior = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <remarks>
        /// Con el constructor predeterminado no se establece el comportamiento para los encabezados.
        /// </remarks>
        public MissingPayloadHeader()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MissingHeadersManager"/>.
        /// </summary>
        /// <param name="payloadHeaderBehavior">Expresión que determina el comportamiento del valor para la cabecera del payload.</param>
        public MissingPayloadHeader(Func<string> payloadHeaderBehavior)
        {
            this.payloadHeaderBehavior = payloadHeaderBehavior;
        }

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
            if (this.payloadHeaderBehavior == null)
            {
                return;
            }

            string payloadHeaderValue = this.payloadHeaderBehavior?.Invoke();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, payloadHeaderValue);
        }
    }
}