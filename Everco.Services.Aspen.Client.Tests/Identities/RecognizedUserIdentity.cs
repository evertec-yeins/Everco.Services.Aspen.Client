// -----------------------------------------------------------------------
// <copyright file="RecognizedUserIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Identity;

    /// <summary>
    /// Implementa la información de la identidad de un usuario de pruebas para autenticar las solicitudes en el servicio Aspen.
    /// </summary>
    internal class RecognizedUserIdentity : IUserIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IUserIdentity helperIdentity = null;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IUserIdentity masterIdentity = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RecognizedUserIdentity" />.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <param name="password">La clave de acceso del usuario.</param>
        /// <param name="deviceInfo">La información del dispositivo.</param>
        public RecognizedUserIdentity(string docType, string docNumber, string password, IDeviceInfo deviceInfo = null)
        {
            this.DocType = docType;
            this.DocNumber = docNumber;
            this.Password = password;
            this.Device = deviceInfo ?? DeviceInfo.Current;
        }

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="RecognizedUserIdentity" />.
        /// </summary>
        private RecognizedUserIdentity()
        {
            this.DocType = "CC";
            this.DocNumber = "52080323";
            this.Password = "colombia";
            this.Device = DeviceInfo.Current;
        }

        /// <summary>
        /// Obtiene la identidad de un usuario para fines de comparación.
        /// </summary>
        public static IUserIdentity Helper =>
            helperIdentity ?? (helperIdentity = new RecognizedUserIdentity(
                                      "CC",
                                      "79483129",
                                      "colombia"));

        /// <summary>
        /// Obtiene la identidad de un usuario para la autenticación de las pruebas automatizadas.
        /// </summary>
        public static IUserIdentity Master => masterIdentity ?? (masterIdentity = new RecognizedUserIdentity());

        /// <summary>
        /// Obtiene la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        public IDeviceInfo Device { get; }

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