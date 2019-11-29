// -----------------------------------------------------------------------
// <copyright file="IUserIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-29 12:47 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define la información que se utiliza para autenticar la solicitud en función de un usuario en el servicio Aspen.
    /// </summary>
    public interface IUserIdentity
    {
        /// <summary>
        /// Obtiene la información del dispositivo asociado al usuario que intenta autenticar la solicitud.
        /// </summary>
        IDeviceInfo Device { get; }

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