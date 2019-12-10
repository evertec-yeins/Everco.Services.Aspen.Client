// -----------------------------------------------------------------------
// <copyright file="CachePolicy.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-23 02:16 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Define el uso de la cache para los tokens de autenticación.
    /// </summary>
    internal enum CachePolicy
    {
        /// <summary>
        /// Establece que no se utilice ninguna entrada almacenada en la cache.
        /// </summary>
        BypassCache,

        /// <summary>
        /// Establece que se utilice la cache si el recurso está disponible; de lo contrario, envía la solicitud y almacena en la cache el token de autenticación.
        /// </summary>
        CacheIfAvailable
    }
}