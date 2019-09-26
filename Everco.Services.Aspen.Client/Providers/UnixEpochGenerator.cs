// -----------------------------------------------------------------------
// <copyright file="UnixEpochGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Implementa un generador de épocas o marcas de tiempo Unix (número de segundos que han transcurrido desde 1970-01-01T00:00:00Z)
    /// </summary>
    public class UnixEpochGenerator : IEpochGenerator
    {
        /// <summary>
        /// Obtiene el nombre con el que se agrega esta información a la solicitud.
        /// </summary>
        public string Name => "Epoch";

        /// <summary>
        /// Obtiene el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z
        /// </summary>
        /// <returns>
        /// Marca de tiempo Unix, expresado como el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z
        /// </returns>
        public double GetSeconds() => DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}