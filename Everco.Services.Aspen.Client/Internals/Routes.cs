// -----------------------------------------------------------------------
// <copyright file="Routes.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-19 07:40 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Facilita la definición de endpoints del servicio Aspen.
    /// </summary>
    internal static partial class Routes
    {
        /// <summary>
        /// Endpoint raíz para aplicaciones autónomas.
        /// </summary>
        internal static string AutonomousRoot => "/app";

        /// <summary>
        /// Endpoint raíz para aplicaciones delegadas.
        /// </summary>
        internal static string DelegatedRoot => "/me";
    }
}