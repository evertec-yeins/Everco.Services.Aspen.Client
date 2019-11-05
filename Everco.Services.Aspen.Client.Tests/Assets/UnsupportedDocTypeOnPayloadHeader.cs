// -----------------------------------------------------------------------
// <copyright file="UnsupportedDocTypeOnPayloadHeader.cs" company="Evertec Colombia">
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
    internal class UnsupportedDocTypeOnPayloadHeader : MissingHeadersManager
    {
        /// <summary>
        /// Para uso interno de la prueba.
        /// </summary>
        private readonly Dictionary<string, object> payload = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHeadersManager"/> class.
        /// </summary>
        public UnsupportedDocTypeOnPayloadHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedDocTypeOnPayloadHeader"/> class.
        /// </summary>
        /// <param name="docType">Type of the document.</param>
        public UnsupportedDocTypeOnPayloadHeader(string docType)
        {
            this.payload.Add("DocType", docType);
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
            IDeviceInfo deviceInfo = userIdentity.DeviceInfo ?? new DeviceInfo();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());
            this.payload.Add(ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce());
            this.payload.Add(ServiceLocator.Instance.EpochGenerator.Name, ServiceLocator.Instance.EpochGenerator.GetSeconds());
            this.payload.Add("DocNumber", userIdentity.DocNumber);
            this.payload.Add("Password", userIdentity.Password);
            this.payload.Add("DeviceId", deviceInfo.DeviceId);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwtEncoder.Encode(payload, apiSecret));
        }
    }
}
