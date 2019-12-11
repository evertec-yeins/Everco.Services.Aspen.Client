// -----------------------------------------------------------------------
// <copyright file="EnvironmentEndpoint.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
// ReSharper disable ClassNeverInstantiated.Global
namespace Everco.Services.Aspen.Client.Identity
{
    using System;

    /// <summary>
    /// Obtiene la información que se utiliza para conectar con el servicio Aspen a partir de la configuración de variables de ambiente.
    /// </summary>
    public class EnvironmentEndpoint : EndpointProviderBase
    {
        /// <summary>Inicializa una nueva instancia de la clase <see cref="T:Everco.Services.Aspen.Client.Identity.EnvironmentEndpointProvider"/></summary>
        /// <param name="urlEnvName">Nombre de la variable de ambiente de donde se obtiene el valor de la URL del servicio.</param>
        /// <param name="timeoutEnvName">Nombre de la variable de ambiente de donde se obtiene el valor de timeout del servicio.</param>
        public EnvironmentEndpoint(
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

            this.SetUrl(Environment.GetEnvironmentVariable(urlEnvName));
            this.SetTimeout(Environment.GetEnvironmentVariable(timeoutEnvName), nameof(timeoutEnvName));
        }
    }
}