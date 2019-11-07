// -----------------------------------------------------------------------
// <copyright file="DelegatedAppIdentity.cs" company="Evertec Colombia">
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
    internal class DelegatedAppIdentity : IAppIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static DelegatedAppIdentity @default = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="AutonomousAppIdentity"/> class from being created.
        /// </summary>
        private DelegatedAppIdentity()
        {
        }

        /// <summary>
        /// Obtiene la instancia predeterminada.
        /// </summary>
        public static DelegatedAppIdentity Default => @default ?? (@default = new DelegatedAppIdentity());

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey => "29b35be3-3159-4800-807e-cde138439378";

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret => "colombia";
    }
}