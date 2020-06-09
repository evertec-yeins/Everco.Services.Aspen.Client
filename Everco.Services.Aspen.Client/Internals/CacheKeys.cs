// -----------------------------------------------------------------------
// <copyright file="CacheKeys.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-06-09 09:13 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Define los identificadores o claves para las entradas administradas por caché.
    /// </summary>
    internal class CacheKeys
    {
        /// <summary>
        /// Identificador de la entrada en el caché de la información de configuración soportada por una aplicación.
        /// </summary>
        internal const string AppMovSettings = "APP_MOV_SETTINGS";

        /// <summary>
        /// Identificador de la entrada en el caché de la información del token de autenticación emitido.
        /// </summary>
        internal const string CurrentAuthToken = "CURRENT_AUTH_TOKEN";

        /// <summary>
        /// Identificador de la entrada en el caché de la información asociada con el dispositivo actual del usuario.
        /// </summary>
        internal const string CurrentDevice = "CURRENT_DEVICE_INFO";
    }
}