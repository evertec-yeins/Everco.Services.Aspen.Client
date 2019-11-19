// -----------------------------------------------------------------------
// <copyright file="IPayloadClaimsManager.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-05 13:48 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// Define las operaciones necesarias para establecer las reclamaciones personalizadas en la carga útil para las solicitudes al servicio Aspen.
    /// </summary>
    public interface IPayloadClaimsManager
    {
        /// <summary>
        /// Agrega la reclamación del identificador del dispositivo que origina la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="deviceId">El valor del identificador del dispositivo.</param>
        void AddDeviceIdClaim(IDictionary<string, object> payload, string deviceId);

        /// <summary>
        /// Agrega la reclamación del número de documento asociado al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="docNumber">El valor del número de documento.</param>
        void AddDocNumberClaim(IDictionary<string, object> payload, string docNumber);

        /// <summary>
        /// Agrega la reclamación del tipo de documento asociado al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="docType">El valor del tipo de documento.</param>
        void AddDocTypeClaim(IDictionary<string, object> payload, string docType);

        /// <summary>
        /// Agrega la reclamación para la época o marca de tiempo unix de la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="epoch">El valor de la época o marca de tiempo unix.</param>
        void AddEpochClaim(IDictionary<string, object> payload, double epoch);

        /// <summary>
        /// Agrega la reclamación del identificador aleatorio de un único uso de la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="nonce">El valor del identificador un único uso.</param>
        void AddNonceClaim(IDictionary<string, object> payload, string nonce);

        /// <summary>
        /// Agrega la reclamación de la contraseña o secreto de acceso asociada al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="password">La contraseña del usuario.</param>
        void AddPasswordClaim(IDictionary<string, object> payload, string password);

        /// <summary>
        /// Agrega la reclamación del token de autenticación emitido para la aplicación.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="token">El valor del token de autenticación.</param>
        void AddTokenClaim(IDictionary<string, object> payload, string token);

        /// <summary>
        /// Agrega la reclamación del nombre que identifica al usuario en el servicio.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="username">El valor del nombre de usuario.</param>
        void AddUsernameClaim(IDictionary<string, object> payload, string username);
    }
}