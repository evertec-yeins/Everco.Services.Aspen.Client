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
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen.
    /// </summary>
    internal class AutonomousAppIdentity : IAppIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity @default = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="AutonomousAppIdentity"/> class from being created.
        /// </summary>
        private AutonomousAppIdentity()
        {
        }

        /// <summary>
        /// Obtiene la instancia predeterminada.
        /// </summary>
        public static IAppIdentity Default => @default ?? (@default = new AutonomousAppIdentity());

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey => "1ea2e59d-0e04-4e53-883c-d8387e23443e";

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret => "colombia";
    }
}