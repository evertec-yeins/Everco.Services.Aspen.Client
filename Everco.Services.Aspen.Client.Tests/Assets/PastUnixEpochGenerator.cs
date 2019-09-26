// -----------------------------------------------------------------------
// <copyright file="PastUnixEpochGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-25 07:39 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using Providers;

    /// <summary>
    /// Implementa un generador de épocas o marcas de tiempo Unix (número de segundos que han transcurrido desde 1970-01-01T00:00:00Z)
    /// </summary>
    public class PastUnixEpochGenerator : IEpochGenerator
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
        public double GetSeconds() => DateTimeOffset.Now.AddDays(-3).ToUnixTimeSeconds();
    }
}