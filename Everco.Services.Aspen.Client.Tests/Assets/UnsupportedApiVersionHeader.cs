// -----------------------------------------------------------------------
// <copyright file="NullOrEmptyApiVersionHeader.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using Auth;
    using JWT;
    using Providers;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class UnsupportedApiVersionHeader : IHeadersManager
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HeadersManager"/>
        /// </summary>
        /// <param name="apiVersion">Número de versión para incluir en las cabeceras personalizadas.</param>
        public UnsupportedApiVersionHeader(string apiVersion = null)
        {
            this.ApiVersion = apiVersion;
        }

        /// <summary>
        /// Obtiene el número de versión del API.
        /// </summary>
        public Version RequestedApiVersion => null;

        /// <summary>
        /// Gets the header value behavior.
        /// </summary>
        public string ApiVersion { get; }

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public virtual void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.RequestedApiVersionHeaderName, this.ApiVersion);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a la aplicación en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public virtual void AddSigninPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds() }
            };

            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwtEncoder.Encode(payload, apiSecret));
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para firmar una solicitud de una aplicación a partir del token de autenticación.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para la aplicación.</param>
        public virtual void AddSignedPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            string token)
        {
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="userIdentity">La información que se utiliza para autenticar la solicitud en función de un usuario.</param>
        public virtual void AddSigninPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            IUserIdentity userIdentity)
        {
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para el usuario.</param>
        /// <param name="username">La identificación del usuario autenticado.</param>
        public void AddSignedPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            string token,
            string username)
        {
        }
    }
}
