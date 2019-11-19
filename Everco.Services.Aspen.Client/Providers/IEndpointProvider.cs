// -----------------------------------------------------------------------
// <copyright file="IEndpointProvider.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Define la Url base del servicio Aspen con el que se requiere conectar y el tiempo de espera para las respuestas.
    /// </summary>
    public interface IEndpointProvider
    {
        /// <summary>
        /// Obtiene la URL base para las solicitudes hacia al API de ASPEN.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Obtiene el tiempo de espera para las respuestas del servicio Aspen.
        /// </summary>
        TimeSpan Timeout { get; }
    }
}