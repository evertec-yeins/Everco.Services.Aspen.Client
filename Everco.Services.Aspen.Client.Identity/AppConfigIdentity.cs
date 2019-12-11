// -----------------------------------------------------------------------
// <copyright file="AppConfigIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 11:23 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Obtiene  la información que se utiliza para autenticar la solicitud en el servicio Aspen de la sección appSettings de un archivo de configuración XML.
    /// </summary>
    public class AppConfigIdentity : IAppIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AppConfigIdentity"/>
        /// </summary>
        /// <param name="apiKeyName">Nombre de la entrada en el archivo de configuración XML de donde se obtiene el valor del ApiKey.</param>
        /// <param name="apiSecretName">Nombre de la entrada en el archivo de configuración XML de donde se obtiene el valor del ApiSecret.</param>
        /// <exception cref="ArgumentNullException">appKeyName o appSecretName son nulas, vacías o están compuestas solo de caracteres en blanco.</exception>
        public AppConfigIdentity(
            string apiKeyName = "ASPEN:APIKEY", 
            string apiSecretName = "ASPEN:APISECRET")
        {
            if (string.IsNullOrWhiteSpace(apiKeyName))
            {
                throw new ArgumentNullException(nameof(apiKeyName));
            }

            if (string.IsNullOrWhiteSpace(apiSecretName))
            {
                throw new ArgumentNullException(nameof(apiSecretName));
            }

            this.ApiKey = ConfigurationManager.AppSettings[apiKeyName].TryDecrypt();
            this.ApiSecret = ConfigurationManager.AppSettings[apiSecretName].TryDecrypt();

            if (string.IsNullOrWhiteSpace(this.ApiKey))
            {
                throw new IdentityException($"Value for ApiKey not found in AppSettings:{apiKeyName}.");
            }

            if (string.IsNullOrWhiteSpace(this.ApiSecret))
            {
                throw new IdentityException($"Value for ApiSecret not found in AppSettings:{apiSecretName}.");
            }
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