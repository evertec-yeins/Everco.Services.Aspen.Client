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

    public class EnvironmentIdentity : IAppIdentity
    {
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