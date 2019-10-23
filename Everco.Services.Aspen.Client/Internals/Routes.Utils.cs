// -----------------------------------------------------------------------
// <copyright file="Routes.Utils.cs" company="Evertec Colombia">
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
        internal static class Utils
        {
            /// <summary>
            /// Endpoint del utilitario para registrar las excepciones que se produzcan por cierres inesperados de una aplicación móvil.
            /// </summary>
            public static string AppCrash => $"{Root}/app/crash";

            /// <summary>
            /// Endpoint del utilitario para cifrar cadenas.
            /// </summary>
            public static string Crypto => $"{Root}/crypto";

            /// <summary>
            /// Endpoint para obtener los tipos de documentos de una aplicación.
            /// </summary>
            public static string DocTypes => $"{Root}/document-types";

            /// <summary>
            /// Endpoint raíz para las operaciones de recursos.
            /// </summary>
            private static string Root => "/utils";
        }
    }
}