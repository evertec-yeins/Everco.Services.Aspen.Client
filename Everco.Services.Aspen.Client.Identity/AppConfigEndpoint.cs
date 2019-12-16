// -----------------------------------------------------------------------
// <copyright file="AppConfigEndpoint.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-10 02:53 PM</date>
// ----------------------------------------------------------------------
// ReSharper disable UnusedMember.Global
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Obtiene la información que se utiliza para conectar con el servicio Aspen a partir de las entradas en una sección appSettings de un archivo de configuración XML
    /// </summary>
    public class AppConfigEndpoint : EndpointProviderBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AppConfigEndpoint"/>
        /// </summary>
        /// <param name="urlEnvName">Nombre de la variable de ambiente de donde se toma el valor para la URL del servicio.</param>
        /// <param name="timeoutEnvName">Nombre de la variable de ambiente de donde se toma el valor para el periodo de tiempo del servicio.</param>
        public AppConfigEndpoint(
            string urlEnvName = "ASPEN:SERVICE_URL",
            string timeoutEnvName = "ASPEN:SERVICE_TIMEOUT")
        {
            if (string.IsNullOrWhiteSpace(urlEnvName))
            {
                throw new ArgumentNullException(nameof(urlEnvName));
            }

            if (string.IsNullOrWhiteSpace(timeoutEnvName))
            {
                throw new ArgumentNullException(nameof(timeoutEnvName));
            }

            this.SetUrl(ConfigurationManager.AppSettings[urlEnvName]);
            this.SetTimeout(ConfigurationManager.AppSettings[timeoutEnvName], nameof(timeoutEnvName));
        }
    }
}