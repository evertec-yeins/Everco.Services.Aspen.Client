// -----------------------------------------------------------------------
// <copyright file="InvalidHeadersManager.cs" company="Evertec Colombia">
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
    /// Implementa un manejador que establece comportamientos de prueba para las cabeceras personalizadas esperadas por el servicio.
    /// </summary>
    internal abstract class InvalidHeadersManager : IHeadersManager
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        protected InvalidHeadersManager()
        {
            this.HeaderBehavior = null;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="headerBehavior">El comportamiento del encabezado.</param>
        protected InvalidHeadersManager(Func<string> headerBehavior)
        {
            this.HeaderBehavior = headerBehavior;
        }

        /// <summary>
        /// Obtiene el comportamiento del encabezado.
        /// </summary>
        public Func<string> HeaderBehavior { get; }

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public virtual void AddApiKeyHeader(IRestRequest request, string apiKey)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);
        }

        /// <summary>
        /// Agrega la cabecera que identifica el número de versión del API solicitada.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        public virtual void AddApiVersionHeader(IRestRequest request, string apiVersion)
        {
            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                return;
            }

            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiVersionHeaderName, apiVersion);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para firmar una solicitud de una aplicación a partir del token de autenticación.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para la aplicación.</param>
        public virtual void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token)
        {
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
        public virtual void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token, string username)
        {
            IDeviceInfo deviceInfo = new DeviceInfo();
            Dictionary<string, object> payload = new Dictionary<string, object>
                                                     {
                                                         { ServiceLocator.Instance.PayloadClaimNames.TokenClaimName, token },
                                                         { ServiceLocator.Instance.PayloadClaimNames.UsernameClaimName, username },
                                                         { ServiceLocator.Instance.PayloadClaimNames.DeviceIdClaimName, deviceInfo.DeviceId }
                                                     };

            ServiceLocator.Instance.PayloadClaimsManager.AddNonceClaim(payload, ServiceLocator.Instance.NonceGenerator.GetNonce());
            ServiceLocator.Instance.PayloadClaimsManager.AddEpochClaim(payload, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            string jwt = jwtEncoder.Encode(payload, apiSecret);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwt);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public virtual void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret)
        {
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
        public virtual void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, IUserIdentity userIdentity)
        {
            IDeviceInfo deviceInfo = userIdentity.DeviceInfo ?? new DeviceInfo();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());

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
