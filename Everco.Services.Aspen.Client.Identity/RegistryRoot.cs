// -----------------------------------------------------------------------
// <copyright file="RegistryRoot.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 12:32 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define la clave en el registro de Windows donde se incia la búsqueda.
    /// </summary>
    public enum RegistryRoot
    {
        /// <summary>
        /// Inicia la búsqueda en la clave HKEY_LOCAL_MACHINE.
        /// </summary>
        LocalMachine,

        /// <summary>
        /// Inicia la búsqueda en la clave HKEY_CURRENT_USER.
        /// </summary>
        CurrentUser,

        /// <summary>
        /// Inicia la búsqueda en la clave HKEY_CLASSES_ROOT.
        /// </summary>
        ClassesRoot
    }
}