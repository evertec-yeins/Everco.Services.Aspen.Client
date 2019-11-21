// -----------------------------------------------------------------------
// <copyright file="EnvironmentIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 10:50 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;

    /// <summary>
    /// Representa la información de la identidad de una aplicación a partir de las variables de ambiente del sistema.
    /// </summary>
    public class EnvironmentIdentity : IAppIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnvironmentIdentity" />.
        /// </summary>
        /// <param name="apiKeyEnvName">Nombre de la variable de ambiente que almacena el valor del ApiKey.</param>
        /// <param name="apiSecretEnvName">Nombre de la variable de ambiente que almacena el valor del ApiSecret.</param>
        /// <exception cref="ArgumentNullException">
        /// Se produce cuando <paramref name="apiKeyEnvName"/> o <paramref name="apiSecretEnvName"/> son nulos o vacíos.
        /// </exception>
        public EnvironmentIdentity(string apiKeyEnvName, string apiSecretEnvName)
        {
            if (string.IsNullOrWhiteSpace(apiKeyEnvName))
            {
                throw new ArgumentNullException(nameof(apiKeyEnvName));
            }

            if (string.IsNullOrWhiteSpace(apiSecretEnvName))
            {
                throw new ArgumentNullException(nameof(apiSecretEnvName));
            }

            this.ApiKey = Environment.GetEnvironmentVariable(apiKeyEnvName);
            this.ApiSecret = Environment.GetEnvironmentVariable(apiSecretEnvName);
        }
        
        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret { get; }
    }
}