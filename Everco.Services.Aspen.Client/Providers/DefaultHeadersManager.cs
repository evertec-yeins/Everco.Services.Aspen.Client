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
    public class DefaultHeadersManager : IHeadersManager
    {
        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para incluir en la cabecera.</param>
        public void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            Throw.IfNull(request, nameof(request));
            Throw.IfNullOrEmpty(apiKey, nameof(apiKey));
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);
        }

        /// <summary>
        /// Agrega la cabecera que identifica el número de versión del API solicitada.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        public void AddApiVersionHeader(IRestRequest request, string apiVersion)
        {
            Throw.IfNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                return;
            }

            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, Version.Parse(apiVersion).ToString());
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para firmar una solicitud de una aplicación a partir del token de autenticación.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para la aplicación.</param>
        public void AddSignedPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            string token)
        {
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Throw.IfNullOrEmpty(token, nameof(token));
            Dictionary<string, object> payload = new Dictionary<string, object>();
            ServiceLocator.Instance.PayloadClaimsManager.AddNonceClaim(payload, ServiceLocator.Instance.NonceGenerator.GetNonce());
            ServiceLocator.Instance.PayloadClaimsManager.AddEpochClaim(payload, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            ServiceLocator.Instance.PayloadClaimsManager.AddTokenClaim(payload, token);
            string jwt = jwtEncoder.Encode(payload, apiSecret);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwt);
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
                { ServiceLocator.Instance.PayloadClaimNames.UsernameClaimName, username },
            };

            ServiceLocator.Instance.PayloadClaimsManager.AddNonceClaim(payload, ServiceLocator.Instance.NonceGenerator.GetNonce());
            ServiceLocator.Instance.PayloadClaimsManager.AddEpochClaim(payload, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            ServiceLocator.Instance.PayloadClaimsManager.AddTokenClaim(payload, token);
            ServiceLocator.Instance.PayloadClaimsManager.AddDeviceIdClaim(payload, deviceInfo.DeviceId);

            string jwt = jwtEncoder.Encode(payload, apiSecret);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwt);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a la aplicación en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public void AddSigninPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret)
        {
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(jwtEncoder, nameof(jwtEncoder));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
            Dictionary<string, object> payload = new Dictionary<string, object>();
            ServiceLocator.Instance.PayloadClaimsManager.AddNonceClaim(payload, ServiceLocator.Instance.NonceGenerator.GetNonce());
            ServiceLocator.Instance.PayloadClaimsManager.AddEpochClaim(payload, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            string jwt = jwtEncoder.Encode(payload, apiSecret);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwt);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="userIdentity">La información que se utiliza para autenticar la solicitud en función de un usuario.</param>
        public void AddSigninPayloadHeader(
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
                { ServiceLocator.Instance.PayloadClaimNames.PasswordClaimName, userIdentity.Password },
            };

            ServiceLocator.Instance.PayloadClaimsManager.AddNonceClaim(payload, ServiceLocator.Instance.NonceGenerator.GetNonce());
            ServiceLocator.Instance.PayloadClaimsManager.AddEpochClaim(payload, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            ServiceLocator.Instance.PayloadClaimsManager.AddDocTypeClaim(payload, userIdentity.DocType);
            ServiceLocator.Instance.PayloadClaimsManager.AddDocNumberClaim(payload, userIdentity.DocNumber);
            ServiceLocator.Instance.PayloadClaimsManager.AddDeviceIdClaim(payload, deviceInfo.DeviceId);
            string jwt = jwtEncoder.Encode(payload, apiSecret);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwt);
        }
    }
}