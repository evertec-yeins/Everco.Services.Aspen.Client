// -----------------------------------------------------------------------
// <copyright file="IdentityException.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-26 02:04 PM</date>
// ----------------------------------------------------------------------
// ReSharper disable MemberCanBePrivate.Global
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Se genera cuando se encuentra un problema al procesar una instancia de identidad.
    /// </summary>
    public class IdentityException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="IdentityException"/>.
        /// </summary>
        public IdentityException()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="IdentityException"/>
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public IdentityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="IdentityException"/>
        /// </summary>
        /// <param name="message">El mensaje de error que explica el motivo de la excepción.</param>
        /// <param name="innerException">La excepción que es la causa de la excepción actual o una referencia nula si no se especifica una excepción interna.</param>
        public IdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="IdentityException"/>
        /// </summary>
        /// <param name="innerException">La excepción que es la causa de la excepción actual o una referencia nula si no se especifica una excepción interna.</param>
        public IdentityException(Exception innerException) : this(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="IdentityException"/>
        /// </summary>
        /// <param name="info">Instancia de <see cref="SerializationInfo" /> que contiene los datos del objeto serializado sobre la excepción lanzada.</param>
        /// <param name="context">Instancia de <see cref="StreamingContext" /> que contiene información contextual sobre la fuente o el destino.</param>
        protected IdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}