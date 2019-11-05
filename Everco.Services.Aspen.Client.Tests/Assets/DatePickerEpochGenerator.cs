// -----------------------------------------------------------------------
// <copyright file="DatePickerEpochGenerator.cs" company="Evertec Colombia">
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
    public class DatePickerEpochGenerator : IEpochGenerator
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly int days = 0;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DatePickerEpochGenerator"/>.
        /// </summary>
        /// <param name="days">El número de días de se agregarán o restarán a la época. El número puede ser negativo o positivo.</param>
        public DatePickerEpochGenerator(int days)
        {
            this.days = days;
        }

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
        public double GetSeconds() => DateTimeOffset.Now.AddDays(this.days).ToUnixTimeSeconds();
    }
}