// -----------------------------------------------------------------------
// <copyright file="Routes.Resources.cs" company="Evertec Colombia">
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
        internal static class Resources
        {
            /// <summary>
            /// Endpoint para obtener los tipos de documentos de una aplicación.
            /// </summary>
            public static string DocTypes => $"{Root}/document-types";

            /// <summary>
            /// Endpoint para obtener los tipos de pago de una aplicación.
            /// </summary>
            public static string PaymentTypes => $"{Root}/payment-types";

            /// <summary>
            /// Endpoint para obtener los operadores de telefonía móvil de una aplicación.
            /// </summary>
            public static string Telcos => $"{Root}/telcos";

            /// <summary>
            /// Endpoint para obtener los valores admitidos para los procesos de recargas de celulares de una aplicación.
            /// </summary>
            public static string TopUp => $"{Root}/topups";

            /// <summary>
            /// Endpoint para obtener los tipos de transacción de una aplicación.
            /// </summary>
            public static string TranTypes => $"{Root}/tran-types";

            /// <summary>
            /// Endpoint para obtener el menú de una aplicación.
            /// </summary>
            public static string Menu => $"{Root}/menu";

            /// <summary>
            /// Endpoint para obtener los valores misceláneos de la aplicación.
            /// </summary>
            public static string Miscellaneous => $"{Root}/miscs";

            /// <summary>
            /// Endpoint raíz para las operaciones de recursos.
            /// </summary>
            private static string Root => "/resx";
        }
    }
}