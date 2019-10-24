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
    internal class MissingPayloadHeader : IHeadersManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingApiKeyHeader"/> class.
        /// </summary>
        public MissingPayloadHeader()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingApiKeyHeader"/> class.
        /// </summary>
        /// <param name="headerValueBehavior">The header value behavior.</param>
        public MissingPayloadHeader(HeaderValueBehavior headerValueBehavior)
        {
            this.HeaderValueBehavior = headerValueBehavior;
        }

        /// <summary>
        /// Gets the header value behavior.
        /// </summary>
        public HeaderValueBehavior HeaderValueBehavior { get; }

        /// <summary>
        /// Obtiene el número de versión que se envia en la solicitud.
        /// </summary>
        public Version RequestedApiVersion => null;

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);

            if (this.RequestedApiVersion != null)
            {
                request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, this.RequestedApiVersion.ToString(2));
            }
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret)
        {
            switch (this.HeaderValueBehavior)
            {
                case HeaderValueBehavior.Null:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, null);
                    break;

                case HeaderValueBehavior.Empty:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, string.Empty);
                    break;

                case HeaderValueBehavior.WhiteSpaces:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, "    ");
                    break;

                case HeaderValueBehavior.UnexpectedFormat:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.");
                    break;
            }
        }

        public void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token)
        {
            throw new NotImplementedException();
        }

        public void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, IUserIdentity userIdentity)
        {
            switch (this.HeaderValueBehavior)
            {
                case HeaderValueBehavior.Null:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, null);
                    break;

                case HeaderValueBehavior.Empty:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, string.Empty);
                    break;

                case HeaderValueBehavior.WhiteSpaces:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, "    ");
                    break;

                case HeaderValueBehavior.UnexpectedFormat:
                    request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.");
                    break;
            }
        }

        public void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token, string username)
        {
            throw new NotImplementedException();
        }
    }
}
