// -----------------------------------------------------------------------
// <copyright file="InvalidTokenPayloadClaim.cs" company="Evertec Colombia">
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
    /// Implementa un manejador que establece comportamientos personalizados para fines de pruebas de la reclamación del token de autenticación para la carga útil de las solicitudes.
    /// </summary>
    public class InvalidTokenPayloadClaim : InvalidPayloadClaimsManager
    {
        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="InvalidTokenPayloadClaim" />.
        /// </summary>
        private InvalidTokenPayloadClaim()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="claimBehavior">El comportamiento de la reclamación.</param>
        private InvalidTokenPayloadClaim(Func<object> claimBehavior) : base(claimBehavior)
        {
        }

        /// <summary>
        /// Crea una instancia de <see cref="IPayloadClaimsManager"/> con un comportamiento personalizado para la reclamación de la carga útil de la solicitud.
        /// </summary>
        /// <param name="claimBehavior">El comportamiento de la reclamación.</param>
        /// <returns>Instancia de <see cref="IPayloadClaimsManager"/> con el comportamiento de personalizado.</returns>
        public static IPayloadClaimsManager WithClaimBehavior(Func<object> claimBehavior) => new InvalidTokenPayloadClaim(claimBehavior);

        /// <summary>
        /// Crea una instancia de <see cref="IPayloadClaimsManager"/> donde se evita agregar la reclamación a la carga útil de la solicitud.
        /// </summary>
        /// <returns>Instancia de <see cref="IPayloadClaimsManager"/> con el comportamiento de personalizado.</returns>
        public static IPayloadClaimsManager AvoidingClaim() => new InvalidTokenPayloadClaim();

        /// <summary>
        /// Agrega la reclamación del token de autenticación emitido para la aplicación.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="token">El valor del token de autenticación.</param>
        public override void AddTokenClaim(IDictionary<string, object> payload, string token)
        {
            if (this.ClaimBehavior == null)
            {
                return;
            }

            object tokenClaimValue = this.ClaimBehavior?.Invoke();
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.TokenClaimName, tokenClaimValue);
        }
    }
}