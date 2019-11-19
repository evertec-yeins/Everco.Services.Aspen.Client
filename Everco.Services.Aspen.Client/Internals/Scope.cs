// -----------------------------------------------------------------------
// <copyright file="Scope.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-24 12:23 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Define el acceso permitido para una aplicación.
    /// </summary>    
    internal enum Scope
    {
        /// <summary>
        /// La aplicación puede solicitar datos de cualquier tarjetahabiente.
        /// </summary>
        Autonomous,

        /// <summary>
        /// La aplicación solo puede solicitar datos del tarjetahabiente para el que se emitió el token de autenticación.
        /// </summary>
        Delegated,

        /// <summary>
        /// El acceso está permitido para cualquiera que alcance el recurso.
        /// </summary>
        Anonymous
    }
}