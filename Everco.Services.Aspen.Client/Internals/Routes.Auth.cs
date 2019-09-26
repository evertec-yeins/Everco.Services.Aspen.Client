// -----------------------------------------------------------------------
// <copyright file="Routes.Auth.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-19 07:44 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Facilita la definición de endpoints del servicio Aspen.
    /// </summary>
    internal static partial class Routes
    {
        /// <summary>
        /// Endpoints para procesos de autenticación.
        /// </summary>
        internal static class Auth
        {
            /// <summary>
            /// Endpoint para el proceso de autenticación de aplicaciones autónomas y delegadas.
            /// </summary>
            internal static string Signin => "/auth/signin";
        }
    }
}