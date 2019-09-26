// -----------------------------------------------------------------------
// <copyright file="DelegatedAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Auth;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen.
    /// </summary>
    internal class DelegatedAppIdentity : IAppIdentity, IUserIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static DelegatedAppIdentity @default = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="AutonomousAppIdentity"/> class from being created.
        /// </summary>
        private DelegatedAppIdentity()
        {
        }

        public static DelegatedAppIdentity Default => @default ?? (@default = new DelegatedAppIdentity());

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey => "29b35be3-3159-4800-807e-cde138439378";

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret => "colombia";

        /// <summary>
        /// Obtiene o establece el tipo de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocType => "CC";

        /// <summary>
        /// Obtiene o establece el número de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        public string DocNumber => "52080323";

        /// <summary>
        /// Obtiene o establece la clave de acceso del usuario que intenta autenticar la solicitud.
        /// </summary>
        public string Password => "colombia";

        /// <summary>
        /// Obtiene o establece la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        public IDeviceInfo DeviceInfo => new DeviceInfo();
    }
}