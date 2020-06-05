// -----------------------------------------------------------------------
// <copyright file="IUtilsModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-06-03 16:16 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Anonymous
{
    using System.Threading.Tasks;

    /// <summary>
    /// Define las operaciones soportadas para utilidades varias por el servicio Aspen.
    /// </summary>
    public interface IUtilsModule
    {
        /// <summary>
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación que generó el error.</param>
        /// <param name="username">El identificador del último usuario que uso la aplicación antes de generarse el error.</param>
        /// <param name="errorReport">La información del reporte de error generado en la aplicación.</param>
        void SaveAppCrash(string apiKey, string username, string errorReport);

        /// <summary>
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación que generó el error.</param>
        /// <param name="username">El identificador del último usuario que uso la aplicación antes de generarse el error.</param>
        /// <param name="errorReport">La información del reporte de error generado en la aplicación.</param>
        /// <returns>
        /// Instancia de <see cref="Task" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task SaveAppCrashAsync(string apiKey, string username, string errorReport);
    }
}