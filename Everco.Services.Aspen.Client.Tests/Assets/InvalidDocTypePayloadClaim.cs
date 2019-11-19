// -----------------------------------------------------------------------
// <copyright file="InvalidDocTypePayloadClaim.cs" company="Evertec Colombia">
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
    /// Implementa un manejador que establece comportamientos personalizados para fines de pruebas de la reclamación de la época para la carga útil de las solicitudes.
    /// </summary>
    public class InvalidDocTypePayloadClaim : InvalidPayloadClaimsManager
    {
        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="InvalidDocTypePayloadClaim" />.
        /// </summary>
        private InvalidDocTypePayloadClaim()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InvalidHeadersManager"/>.
        /// </summary>
        /// <param name="claimBehavior">El comportamiento de la reclamación.</param>
        private InvalidDocTypePayloadClaim(Func<object> claimBehavior) : base(claimBehavior)
        {
        }

        /// <summary>
        /// Crea una instancia de <see cref="IPayloadClaimsManager"/> con un comportamiento personalizado para la reclamación de la carga útil de la solicitud.
        /// </summary>
        /// <param name="claimBehavior">El comportamiento de la reclamación.</param>
        /// <returns>Instancia de <see cref="IPayloadClaimsManager"/> con el comportamiento de personalizado.</returns>
        public static IPayloadClaimsManager WithClaimBehavior(Func<object> claimBehavior) => new InvalidDocTypePayloadClaim(claimBehavior);

        /// <summary>
        /// Crea una instancia de <see cref="IPayloadClaimsManager"/> donde se evita agregar la reclamación a la carga útil de la solicitud.
        /// </summary>
        /// <returns>Instancia de <see cref="IPayloadClaimsManager"/> con el comportamiento de personalizado.</returns>
        public static IPayloadClaimsManager AvoidingClaim() => new InvalidDocTypePayloadClaim();

        /// <summary>
        /// Agrega la reclamación del tipo de documento asociado al usuario.
        /// </summary>
        /// <param name="payload">Colección de claves y valores que representa la carga útil para la solicitud.</param>
        /// <param name="docType">El valor del tipo de documento.</param>
        public override void AddDocTypeClaim(IDictionary<string, object> payload, string docType)
        {
            if (this.ClaimBehavior == null)
            {
                return;
            }

            object docTypeClaimValue = this.ClaimBehavior?.Invoke();
            payload.Add(ServiceLocator.Instance.PayloadClaimNames.DocTypeClaimName, docTypeClaimValue);
        }
    }
}