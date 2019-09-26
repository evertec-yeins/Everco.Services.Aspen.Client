// -----------------------------------------------------------------------
// <copyright file="ISettingsModule.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las operaciones soportadas por el servicio Aspen para acceder a entidades de información relacionadas con parametrización del sistema.
    /// </summary>
    public interface ISettingsModule
    {
        /// <summary>
        /// Obtiene la lista de tipos de documento soportados por el servicio Aspen.
        /// </summary>
        /// <returns>Lista de tipos de documento soportados.</returns>
        IList<DocTypeInfo> GetDocTypes();
    }
}