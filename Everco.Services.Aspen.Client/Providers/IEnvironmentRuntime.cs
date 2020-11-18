// -----------------------------------------------------------------------
// <copyright file="IEnvironmentRuntime.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-10-21 10:09 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Proporciona información sobre el entorno en el que se está ejecutando la aplicación.
    /// </summary>
    public interface IEnvironmentRuntime
    {
        /// <summary>
        /// Obtiene el nombre del entorno a partir del valor de la variable de entorno "ASPEN:ENVIRONMENT".
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Obtiene un valor que indica si entorno de alojamiento actual es desarrollo.
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// Obtiene un valor que indica si entorno de alojamiento actual es producción.
        /// </summary>
        bool IsProduction { get; }

        /// <summary>
        /// Obtiene un valor que indica si entorno de alojamiento actual es certificación.
        /// </summary>        
        bool IsStaging { get; }

        /// <summary>
        /// Obtiene un valor que indica si la ambiente se está ejecutando en proceso de pruebas.
        /// </summary>
        bool IsTestingExecuting { get; }
    }
}