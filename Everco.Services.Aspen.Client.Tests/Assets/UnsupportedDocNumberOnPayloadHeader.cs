// -----------------------------------------------------------------------
// <copyright file="UnsupportedDocNumberOnPayloadHeader.cs" company="Evertec Colombia">
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
    internal class UnsupportedDocNumberOnPayloadHeader : IHeadersManager
    {
        /// <summary>
        /// Para uso interno de la prueba.
        /// </summary>
        private readonly Dictionary<string, object> payload = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingApiKeyHeader"/> class.
        /// </summary>
        public UnsupportedDocNumberOnPayloadHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingApiKeyHeader"/> class.
        /// </summary>
        public UnsupportedDocNumberOnPayloadHeader(string docNumber)
        {
            this.payload.Add("DocNumber", docNumber);
        }

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
                request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.RequestedApiVersionHeaderName, this.RequestedApiVersion.ToString(2));
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
            throw new NotImplementedException();
        }

        public void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token)
        {
            throw new NotImplementedException();
        }

        public void AddSigninPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, IUserIdentity userIdentity)
        {
            IDeviceInfo deviceInfo = userIdentity.DeviceInfo ?? new DeviceInfo();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());
            this.payload.Add(ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce());
            this.payload.Add(ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            this.payload.Add("DocType", userIdentity.DocType);
            this.payload.Add("Password", userIdentity.Password);
            this.payload.Add("DeviceId", deviceInfo.DeviceId);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwtEncoder.Encode(payload, apiSecret));
        }

        public void AddSignedPayloadHeader(IRestRequest request, IJwtEncoder jwtEncoder, string apiSecret, string token, string username)
        {
            throw new NotImplementedException();
        }
    }
}
