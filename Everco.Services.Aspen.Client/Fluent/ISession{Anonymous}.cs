// -----------------------------------------------------------------------
// <copyright file="ISession.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    /// <summary>
    /// Define operaciones y propiedades para acceder a los valores actuales de conexión con el servicio ASPEN.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Obtiene la instancia actual configurada para interactuar con el servicio ASPEN.
        /// </summary>
        /// <returns>
        /// Instancia de <seealso cref="IAnonymous"/> que permite interactuar con el servicio ASPEN.
        /// </returns>
        IAnonymous GetClient();
    }
}