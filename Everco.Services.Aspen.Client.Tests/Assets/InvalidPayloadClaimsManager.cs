// -----------------------------------------------------------------------
// <copyright file="InvalidPayloadClaimsManager.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-05 13:48 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using Everco.Services.Aspen.Client.Providers;

    /// <summary>
    /// Implementa las operaciones necesarias para establecer las reclamaciones personalizadas en la carga útil para las solicitudes al servicio Aspen.
    /// </summary>
    public abstract class InvalidPayloadClaimsManager : IPayloadClaimsManager
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FixedEpochGenerator"/>.
        /// </summary>
        protected InvalidPayloadClaimsManager()
        {
            this.ClaimBehavior = null;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FixedEpochGenerator"/>.
        /// </summary>
        /// <param name="claimBehavior">El número de días de se agregarán o restarán a la época. El número puede ser negativo o positivo.</param>
        protected InvalidPayloadClaimsManager(Func<object> claimBehavior)
        {
            this.ClaimBehavior = claimBehavior;
        }

        /// <summary>
        /// Obtiene el comportamiento de la reclamación.
        /// </summary>
        public Func<object> ClaimBehavior { get; }

        /// <summary>
        /// Agrega la reclamación del identificador del dispositivo que origina la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="deviceId">El valor del token de autenticación.</param>
        public virtual void AddDeviceIdClaim(IDictionary<string, object> payload, string deviceId)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.DeviceIdClaimName, deviceId);
        }

        /// <summary>
        /// Agrega la reclamación del número de documento asociado al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="docNumber">El valor del número de documento.</param>
        public virtual void AddDocNumberClaim(IDictionary<string, object> payload, string docNumber)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.DocNumberClaimName, docNumber);
        }

        /// <summary>
        /// Agrega la reclamación del tipo de documento asociado al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="docType">El valor del tipo de documento.</param>
        public virtual void AddDocTypeClaim(IDictionary<string, object> payload, string docType)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.DocTypeClaimName, docType);
        }

        /// <summary>
        /// Agrega la reclamación para la época o marca de tiempo unix de la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="epoch">El valor de la época o marca de tiempo unix.</param>
        public virtual void AddEpochClaim(IDictionary<string, object> payload, double epoch)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.EpochClaimName, epoch);
        }

        /// <summary>
        /// Agrega la reclamación del identificador aleatorio de un único uso para la solicitud.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="nonce">El valor del identificador un único uso.</param>
        public virtual void AddNonceClaim(IDictionary<string, object> payload, string nonce)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.NonceClaimName, nonce);
        }

        /// <summary>
        /// Agrega la reclamación de la contraseña o secreto de acceso asociada al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="password">La contraseña del usuario.</param>
        public virtual void AddPasswordClaim(IDictionary<string, object> payload, string password)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.PasswordClaimName, password);
        }

        /// <summary>
        /// Agrega la reclamación del token de autenticación emitido para la aplicación.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="token">El valor del token de autenticación.</param>
        public virtual void AddTokenClaim(IDictionary<string, object> payload, string token)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.TokenClaimName, token);
        }

        /// <summary>
        /// Agrega la reclamación del nombre que identifica al usuario en el servicio.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="username">El valor del nombre de usuario.</param>
        public virtual void AddUsernameClaim(IDictionary<string, object> payload, string username)
        {
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.UsernameClaimName, username);
        }
    }
}