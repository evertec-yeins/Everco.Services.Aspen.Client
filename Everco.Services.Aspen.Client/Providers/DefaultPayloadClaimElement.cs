// -----------------------------------------------------------------------
// <copyright file="DefaultPayloadClaimElement.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-01 17:47 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Implementa los nombres que se utilizan para las reclamaciones usadas en la carga útil de una solicitud al servicio Aspen.
    /// </summary>
    public class DefaultPayloadClaimElement : IPayloadClaimElement
    {
        /// <summary>
        /// Obtiene el nombre de la reclamación para el identificador de dispositivo.
        /// </summary>
        public string DeviceIdClaimName => "DeviceId";

        /// <summary>
        /// Obtiene el nombre de la reclamación para el número de documento asociado al usuario.
        /// </summary>
        public string DocNumberClaimName => "DocNumber";

        /// <summary>
        /// Obtiene el nombre de la reclamación para el tipo de documento asociado al usuario.
        /// </summary>
        public string DocTypeClaimName => "DocType";

        /// <summary>
        /// Obtiene el nombre de la reclamación para la época unix de la solicitud.
        /// </summary>
        public string EpochClaimName => "Epoch";

        /// <summary>
        /// Obtiene el nombre de la reclamación para el número o cadena aleatoria de un único uso que identifica a la solicitud.
        /// </summary>
        public string NonceClaimName => "Nonce";

        /// <summary>
        /// Obtiene el nombre de la reclamación para la contraseña asociada a la autenticación del usuario.
        /// </summary>
        public string PasswordClaimName => "Password";

        /// <summary>
        /// Obtiene el nombre de la reclamación para el token de autenticación de la solicitud.
        /// </summary>
        public string TokenClaimName => "Token";

        /// <summary>
        /// Obtiene el nombre de la reclamación para el nombre del usuario.
        /// </summary>
        public string UsernameClaimName => "Username";
    }
}