// -----------------------------------------------------------------------
// <copyright file="AutonomousAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
// ReSharper disable MemberCanBePrivate.Global
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Everco.Services.Aspen.Client.Identity;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen para defectos de las pruebas automáticas.
    /// </summary>
    internal class AutonomousAppIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity assistantIdentity;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity masterIdentity;

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
        /// Obtiene la identidad de una aplicación autónoma para fines de comparación.
        /// </summary>
        public static IAppIdentity Helper =>
            assistantIdentity ?? (assistantIdentity = new EnvironmentIdentity(
                "ASPEN:AUTONOMOUSAPP2:APIKEY",
                "ASPEN:AUTONOMOUSAPP2:APISECRET"));

        /// <summary>
        /// Obtiene la identidad de una aplicación autónoma para la autenticación de las pruebas automatizadas.
        /// </summary>
        public static IAppIdentity Master => masterIdentity ?? (masterIdentity = new EnvironmentIdentity(
            "ASPEN:AUTONOMOUSAPP1:APIKEY",
            "ASPEN:AUTONOMOUSAPP1:APISECRET"));

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