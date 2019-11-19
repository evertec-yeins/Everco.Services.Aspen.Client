// -----------------------------------------------------------------------
// <copyright file="EnvironmentRuntime.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-10-21 10:09 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Proporciona información sobre el entorno en el que se está ejecutando la aplicación.
    /// </summary>
    internal class EnvironmentRuntime : IEnvironmentRuntime
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnvironmentRuntime"/>
        /// </summary>
        public EnvironmentRuntime()
        {
            string environmentValue = Environment
                .GetEnvironmentVariable("ASPEN:ENVIRONMENT", EnvironmentVariableTarget.Machine)
                .DefaultIfNullOrEmpty(Environment.MachineName);
            this.EnvironmentName = environmentValue.Trim().ToUpper();
        }

        /// <summary>
        /// Obtiene el nombre del entorno a partir del valor de la variable de entorno "ASPEN:ENVIRONMENT".
        /// </summary>
        public string EnvironmentName { get; }

        /// <summary>
        /// Obtiene un valor que indica si entorno de ejecución actual es desarrollo.
        /// </summary>
        public bool IsDevelopment => IsDebugMode() || this.EnvironmentName.StartsWith("DEV");

        /// <summary>
        /// Obtiene un valor que indica si entorno de ejecución actual es producción.
        /// </summary>
        public bool IsProduction => this.EnvironmentName.StartsWith("PRO");

        /// <summary>
        /// Obtiene un valor que indica si entorno de ejecución actual es pruebas/certificación.
        /// </summary>        
        public bool IsStaging => this.EnvironmentName.StartsWith("CERT") || this.EnvironmentName.StartsWith("QA");

        /// <summary>
        /// Retorna true si se el assembly está compilado en modo de depuración.
        /// </summary>
        /// <returns>Modo de depuración.</returns>
        private static bool IsDebugMode()
        {
#pragma warning disable 162
#if DEBUG
            return true;
#endif
            return false;
#pragma warning restore 162
        }
    }
}