// -----------------------------------------------------------------------
// <copyright file="ILoggingProvider.cs" company="Processa"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-26 10:17 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Define un tipo que puede escribir información de seguimiento.
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Escribe información de depuración en una traza de seguimiento.
        /// </summary>
        /// <param name="message">Texto del mensaje de depuración.</param>
        void WriteDebug(string message);

        /// <summary>
        /// Escribe información de un objeto en una traza de seguimiento.
        /// </summary>
        /// <param name="message">Texto del mensaje de información.</param>
        /// <param name="object">Instancia del objeto a escribir.</param>
        void WriteInfo(string message, object @object = null);

        /// <summary>
        /// Escribe información de un error en una traza de seguimiento.
        /// </summary>
        /// <param name="exception">Excepción que ocasion+o el error.</param>
        /// <param name="message">Texto del mensaje de error.</param>
        void WriteError(Exception exception, string message = null);
    }
}