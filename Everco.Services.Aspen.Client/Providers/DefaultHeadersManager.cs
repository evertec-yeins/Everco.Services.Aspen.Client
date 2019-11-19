// -----------------------------------------------------------------------
// <copyright file="DefaultHeadersManager.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;
    using System.Collections.Generic;
    using Auth;
    using Internals;
    using JWT;
    using RestSharp;

    /// <summary>
    /// Implementa las operaciones necesarias para establecer las cabeceras personalizadas requeridas por el servicio Aspen.
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class DefaultHeadersManager : IHeadersManager
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DefaultHeadersManager"/>
        /// </summary>
        public DefaultHeadersManager()
        {
            this.RequestedApiVersion = null;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DefaultHeadersManager"/>
        /// </summary>
        /// <param name="apiVersion">Número de versión para incluir en las cabeceras personalizadas.</param>
        public DefaultHeadersManager(string apiVersion)
        {
            this.RequestedApiVersion = Version.Parse(apiVersion);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DefaultHeadersManager"/>
        /// </summary>
        /// <param name="apiVersion">Número de versión para incluir en las cabeceras personalizadas.</param>
        public DefaultHeadersManager(Version apiVersion)
        {
            this.RequestedApiVersion = apiVersion;
        }

        /// <summary>
        /// Obtiene el número de versión del API que se envía con la solicitud.
        /// </summary>
        public Version RequestedApiVersion { get; }

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public virtual void AddApiKeyHeader(
            IRestRequest request, 
            string apiKey)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);

            if (this.RequestedApiVersion != null)
            {
                request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, this.RequestedApiVersion.ToString(2));
            }
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
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Throw.IfNullOrEmpty(token, nameof(token));
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds() },
                { "Token", token }
            };

            this.AddPayloadHeader(request, jwtEncoder.Encode(payload, apiSecret));
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
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Throw.IfNullOrEmpty(token, nameof(token));
            Throw.IfNullOrEmpty(username, nameof(username));

            IDeviceInfo deviceInfo = CacheStore.GetDeviceInfo() ?? new DeviceInfo();
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds() },
                { "Token", token },
                { "Username", username },
                { "DeviceId", deviceInfo.DeviceId }
            };

            this.AddPayloadHeader(request, jwtEncoder.Encode(payload, apiSecret));
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
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds() }
            };

            this.AddPayloadHeader(request, jwtEncoder.Encode(payload, apiSecret));
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
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Throw.IfNull(userIdentity, nameof(userIdentity));

            IDeviceInfo deviceInfo = userIdentity.DeviceInfo ?? CacheStore.GetDeviceInfo() ?? new DeviceInfo();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());
            CacheStore.SetDeviceInfo(deviceInfo);

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds() },
                { "DocType", userIdentity.DocType },
                { "DocNumber", userIdentity.DocNumber },
                { "Password", userIdentity.Password },
                { "DeviceId", deviceInfo.DeviceId }
            };

            this.AddPayloadHeader(request, jwtEncoder.Encode(payload, apiSecret));
        }

        /// <summary>
        /// Agrega la cabecera con el payload requerido por el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="value">Cadena con el Payload para agregar.</param>
        /// Agrega la cabecera con el payload requerido por el servicio Aspen.
        private void AddPayloadHeader(
            IRestRequest request, 
            string value)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, value);
        }
    }
}