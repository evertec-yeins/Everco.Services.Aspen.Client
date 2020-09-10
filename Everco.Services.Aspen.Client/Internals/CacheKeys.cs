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
        /// Identificador de la entrada en el caché de la lista de operadores de telefonía móvil soportados para la aplicación.
        /// </summary>
        internal const string Carriers = "RESX_CARRIERS";

        /// <summary>
        /// Identificador de la entrada en el caché de la información del token de autenticación emitido.
        /// </summary>
        internal const string CurrentAuthToken = "CURRENT_AUTH_TOKEN";

        /// <summary>
        /// Identificador de la entrada en el caché de la información asociada con el dispositivo actual del usuario.
        /// </summary>
        internal const string CurrentDevice = "CURRENT_DEVICE_INFO";

        /// <summary>
        /// Identificador de la entrada en el caché de la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        internal const string DocTypes = "RESX_DOCUMENT_TYPES";

        /// <summary>
        /// Identificador de la entrada en el caché de la lista de opciones que representan el menú de una aplicación móvil.
        /// </summary>
        internal const string MenuItems = "RESX_MENU_ITEMS";

        /// <summary>
        /// Identificador de la entrada en el caché de la configuración de valores misceláneos soportados para la aplicación.
        /// </summary>
        internal const string MiscellaneousSettings = "RESX_MISCELLANEOUS_SETTINGS";

        /// <summary>
        /// Identificador de la entrada en el caché de los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
        /// </summary>
        internal const string PaymentTypes = "RESX_PAYMENT_TYPES";

        /// <summary>
        /// Identificador de la entrada en el caché de la lista de canales soportados para la generación de tokens o claves transaccionales.
        /// </summary>
        internal const string TokenChannels = "TOKEN_CHANNELS";

        /// <summary>
        /// Identificador de la entrada en el caché de los valores admitidos de recarga por operador soportados para la aplicación.
        /// </summary>
        internal const string TopUpValues = "RESX_TOPUP_VALUES";

        /// <summary>
        /// Identificador de la entrada en el caché de la lista de los tipos de transacción soportados para la aplicación.
        /// </summary>
        internal const string TranTypes = "RESX_TRANSACTION_TYPES";
    }
}