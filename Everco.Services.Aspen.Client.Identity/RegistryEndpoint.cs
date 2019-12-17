// -----------------------------------------------------------------------
// <copyright file="RegistryEndpoint.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-10 02:40 PM</date>
// ----------------------------------------------------------------------
// ReSharper disable UnusedMember.Global
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /// <summary>
    /// Obtiene la información que se utiliza para conectar con el servicio Aspen a partir de la configuración en el registro de Windows.
    /// </summary>
    public class RegistryEndpoint : EndpointProviderBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RegistryEndpoint"/>
        /// </summary>
        /// <param name="root">Clave en el registro de Windows donde se incia la búsqueda.</param>
        /// <param name="subKeyPath">El nombre o la ruta de la subclave donde se buscan los valores.</param>
        /// <param name="urlName">El nombre de la entrada en donde se busca el valor de la URL. Esta cadena no distingue entre mayúsculas y minúsculas.</param>
        /// <param name="timeoutName">El nombre de la entrada en donde se busca el valor del tiempo de espera.</param>
        public RegistryEndpoint(
            RegistryRoot root = RegistryRoot.LocalMachine,
            string subKeyPath = @"SOFTWARE\Aspen\Credentials",
            string urlName = "SERVICE_URL",
            string timeoutName = "SERVICE_TIMEOUT")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subKeyPath))
                {
                    throw new ArgumentNullException(nameof(subKeyPath));
                }

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    throw new PlatformNotSupportedException();
                }

                RegistryKey rootKey = root.ToRegistryKey();
                
                using (RegistryKey registryKey = rootKey.OpenSubKey(subKeyPath))
                {
                    if (registryKey == null)
                    {
                        return;
                    }

                    this.SetUrl(registryKey.GetValue(urlName)?.ToString());
                    this.SetTimeout(registryKey.GetValue(timeoutName)?.ToString(), nameof(timeoutName));
                }
            }
            catch (Exception exception)
            {
                throw new IdentityException(exception);
            }
        }
    }
}