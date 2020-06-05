// -----------------------------------------------------------------------
// <copyright file="ISettingsModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-06-03 16:16 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Anonymous
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Everco.Services.Aspen.Entities;

    /// <summary>
    /// Define las operaciones soportadas por el servicio Aspen para acceder a entidades de información relacionadas con parametrización del sistema.
    /// </summary>
    public interface ISettingsModule
    {
        /// <summary>
        /// Obtiene la configuración de valores de configuración soportados para la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <returns>
        /// Colección de clavesy valores soportados para la aplicación.
        /// </returns>
        AppMovSettings GetAppSettings(string apiKey);

        /// <summary>
        /// Obtiene la configuración de valores de configuración soportados para la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<AppMovSettings> GetAppSettingsAsync(string apiKey);

        /// <summary>
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>Lista de tipos de documento predeterminados.</returns>
        IList<DocTypeInfo> GetDefaultDocTypes();

        /// <summary>
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<DocTypeInfo>> GetDefaultDocTypesAsync();
    }
}