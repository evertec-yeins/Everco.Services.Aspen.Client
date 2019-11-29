// -----------------------------------------------------------------------
// <copyright file="RegistryIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 12:14 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /// <summary>
    /// Obtiene  la información que se utiliza para autenticar la solicitud en el servicio Aspen del registro de Windows.
    /// </summary>
    public class RegistryIdentity: IAppIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RegistryIdentity" />
        /// </summary>
        /// <param name="root">Clave en el registro de Windows donde se incia la búsqueda.</param>
        /// <param name="subKeyPath">El nombre o la ruta de la subclave donde se buscan los valores.</param>
        /// <param name="apiKeyName">El nombre de la entrada en donde se busca el valor del ApiKey. Esta cadena no distingue entre mayúsculas y minúsculas.</param>
        /// <param name="apiSecretName">El nombre de la entrada en donde se busca el valor del ApiSecret.</param>
        public RegistryIdentity(RegistryRoot root, 
                                string subKeyPath, 
                                string apiKeyName = "ASPEN:APIKEY", 
                                string apiSecretName = "ASPEN:APISECRET")
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

#pragma warning disable SA0102

                RegistryKey rootKey = root switch
                {
                    RegistryRoot.LocalMachine => Registry.LocalMachine,
                    RegistryRoot.CurrentUser => Registry.CurrentUser,
                    _ => Registry.ClassesRoot
                };

#pragma warning restore SA0102

                using RegistryKey registryKey = rootKey.OpenSubKey(subKeyPath);
            
                if (registryKey == null)
                {
                    return;
                }

                this.ApiKey = registryKey.GetValue(apiKeyName)?.ToString()?.TryDecrypt();
                this.ApiSecret = registryKey.GetValue(apiSecretName)?.ToString()?.TryDecrypt();
            }
            catch (Exception exception)
            {
                throw new IdentityException(exception);
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