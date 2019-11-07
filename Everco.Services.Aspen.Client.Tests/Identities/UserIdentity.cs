// -----------------------------------------------------------------------
// <copyright file="UserIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Auth;

    /// <summary>
    /// Implementa la información de la identidad de un usuario de pruebas para autenticar las solicitudes en el servicio Aspen.
    /// </summary>
    internal class UserIdentity : IUserIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static UserIdentity @default = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UserIdentity" />.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <param name="password">La clave de acceso del usuario.</param>
        /// <param name="deviceInfo">La información del dispositivo.</param>
        public UserIdentity(string docType, string docNumber, string password, IDeviceInfo deviceInfo = null)
        {
            this.DocType = docType;
            this.DocNumber = docNumber;
            this.Password = password;
            this.DeviceInfo = deviceInfo ?? new DeviceInfo();
        }

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="UserIdentity" />.
        /// </summary>
        private UserIdentity()
        {
            this.DocType = "CC";
            this.DocNumber = "52080323";
            this.Password = "colombia";
            this.DeviceInfo = new DeviceInfo();
        }

        /// <summary>
        /// Obtiene la instancia predeterminada.
        /// </summary>
        public static UserIdentity Default => @default ?? (@default = new UserIdentity());

        /// <summary>
        /// Obtiene o establece la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        public IDeviceInfo DeviceInfo { get; }

        /// <summary>
        /// Obtiene o establece el número de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocNumber { get; }

        /// <summary>
        /// Obtiene o establece el tipo de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocType { get; }

        /// <summary>
        /// Obtiene o establece la clave de acceso del usuario que intenta autenticar la solicitud.
        /// </summary>
        public string Password { get; }
    }
}