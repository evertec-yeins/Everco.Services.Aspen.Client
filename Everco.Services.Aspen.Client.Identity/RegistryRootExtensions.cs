// -----------------------------------------------------------------------
// <copyright file="RegistryRootExtensions.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-17 10:07 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <summary>
    /// Expone métodos de extensión para los tipos <see cref="RegistryRoot"/>.
    /// </summary>
    public static class RegistryRootExtensions
    {
        /// <summary>
        /// Convierte <paramref name="root"/> en su representación de <see cref="RegistryKey"/>.
        /// </summary>
        /// <param name="root">Valor a convertir.</param>
        /// <returns>Instancia de <see cref="RegistryKey"/> que representa <paramref name="root"/>.</returns>
        public static RegistryKey ToRegistryKey(this RegistryRoot root)
        {
            switch (root)
            {
                case RegistryRoot.LocalMachine:
                    return Registry.LocalMachine;

                case RegistryRoot.CurrentUser:
                    return Registry.CurrentUser;

                case RegistryRoot.ClassesRoot:
                    return Registry.ClassesRoot;

                default:
                    throw new InvalidEnumArgumentException(nameof(root), (int)root, typeof(RegistryRoot));
            }
        }
    }
}