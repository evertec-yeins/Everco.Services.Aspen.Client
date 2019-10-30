// -----------------------------------------------------------------------
// <copyright file="IUserIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Auth
{
    /// <summary>
    /// Define la información que se utiliza para autenticar la solicitud en función de un usuario en el servicio Aspen.
    /// </summary>
    public interface IUserIdentity
    {
        /// <summary>
        /// Obtiene la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        IDeviceInfo DeviceInfo { get; }

        /// <summary>
        /// Obtiene el número de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>
        string DocNumber { get; }

        /// <summary>
        /// Obtiene el tipo de documento asociado con el usuario que intenta autenticar la solicitud.
        /// </summary>        
        string DocType { get; }

        /// <summary>
        /// Obtiene la clave de acceso del usuario que intenta autenticar la solicitud.
        /// </summary>
        string Password { get; }
    }
}