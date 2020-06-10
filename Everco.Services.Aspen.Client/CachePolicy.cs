// -----------------------------------------------------------------------
// <copyright file="CachePolicy.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-23 02:16 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client
{
    /// <summary>
    /// Define el uso de la información administrada por caché.
    /// </summary>
    public enum CachePolicy
    {
        /// <summary>
        /// Establece que no se utilice ninguna entrada almacenada en el caché.
        /// </summary>
        BypassCache,

        /// <summary>
        /// Establece que se utilice la cache si la entrada está disponible; de lo contrario, envía la solicitud y almacena la información en el caché.
        /// </summary>
        CacheIfAvailable
    }
}