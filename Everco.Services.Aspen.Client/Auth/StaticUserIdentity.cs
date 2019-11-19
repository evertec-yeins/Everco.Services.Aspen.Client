// -----------------------------------------------------------------------
// <copyright file="StaticUserIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Auth
{
    using System;
    using Internals;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en función de un usuario en el servicio Aspen.
    /// </summary>
    internal class StaticUserIdentity : IUserIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="StaticUserIdentity"/>
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <param name="password">La clave de acceso del usuario.</param>
        /// <param name="deviceInfo">La información del dispositivo desde donde se intenta autenticar el usuario.</param>
        public StaticUserIdentity(string docType, string docNumber, string password, IDeviceInfo deviceInfo = null)
        {
            Throw.IfNullOrEmpty(docType, nameof(docType));
            Throw.IfNullOrEmpty(docNumber, nameof(docNumber));
            Throw.IfNullOrEmpty(password, nameof(password));
            this.DocType = docType;
            this.DocNumber = docNumber;
            this.Password = password;
            this.DeviceInfo = deviceInfo ?? new DeviceInfo();
            string defaultDeviceId = $@"{Environment.UserDomainName}\{Environment.UserName}";
            string deviceId = this.DeviceInfo.DeviceId.DefaultIfNullOrEmpty(defaultDeviceId);
            this.DeviceInfo.DeviceId = deviceId;
        }

        /// <summary>
        /// Obtiene la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        public IDeviceInfo DeviceInfo { get; }

        /// <summary>
        /// Obtiene el número de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocNumber { get; }

        /// <summary>
        /// Obtiene el tipo de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocType { get; }
        
        /// <summary>
        /// Obtiene la clave de acceso del usuario que intenta autenticar la solicitud.
        /// </summary>
        public string Password { get; }
    }
}