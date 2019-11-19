// -----------------------------------------------------------------------
// <copyright file="AutonomousAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Auth;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen para defectos de las pruebas automáticas.
    /// </summary>
    internal class AutonomousAppIdentity : IAppIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity assistantIdentity = null;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity masterIdentity = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AutonomousAppIdentity" />.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <param name="apiSecret">El secreto de la aplicación.</param>
        public AutonomousAppIdentity(string apiKey, string apiSecret)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
        }

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="AutonomousAppIdentity" />.
        /// </summary>
        private AutonomousAppIdentity()
        {
            this.ApiKey = "1ea2e59d-0e04-4e53-883c-d8387e23443e";
            this.ApiSecret = "colombia";
        }

        /// <summary>
        /// Obtiene la identidad de una aplicación autónoma para fines de comparación.
        /// </summary>
        public static IAppIdentity Helper =>
            assistantIdentity ?? (assistantIdentity = new AutonomousAppIdentity(
                                      "c0d8c65e-3445-4fee-b92c-145029101f49",
                                      "colombia"));

        /// <summary>
        /// Obtiene la identidad de una aplicación autónoma para la autenticación de las pruebas automatizadas.
        /// </summary>
        public static IAppIdentity Master => masterIdentity ?? (masterIdentity = new AutonomousAppIdentity());

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret { get; }
    }
}