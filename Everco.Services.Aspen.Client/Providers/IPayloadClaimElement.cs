// -----------------------------------------------------------------------
// <copyright file="IPayloadClaimElement.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-01 17:23 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Define los nombres que se utilizan para las reclamaciones usadas en la carga útil de una solicitud al servicio Aspen.
    /// </summary>
    public interface IPayloadClaimElement
    {
        /// <summary>
        /// Obtiene el nombre de la reclamación para el identificador de dispositivo.
        /// </summary>
        string DeviceIdClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para el número de documento asociado a un usuario.
        /// </summary>
        string DocNumberClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para el tipo de documento asociado a un usuario.
        /// </summary>
        string DocTypeClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para la época unix.
        /// </summary>
        string EpochClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para el identificador aleatorio de un único uso para la solicitud.
        /// </summary>
        string NonceClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para la contraseña de acceso asociada a un usuario.
        /// </summary>
        string PasswordClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para el token de autenticación.
        /// </summary>
        string TokenClaimName { get; }

        /// <summary>
        /// Obtiene el nombre de la reclamación para el nombre asociado al usuario autenticado.
        /// </summary>
        string UsernameClaimName { get; }
    }
}