// -----------------------------------------------------------------------
// <copyright file="FixedEpochGenerator.cs" company="Evertec Colombia">
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
    public class FixedEpochGenerator : IEpochGenerator
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly double seconds;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FixedEpochGenerator"/>.
        /// </summary>
        /// <param name="days">El número de días de se agregarán o restarán a la época. El número puede ser negativo o positivo.</param>
        private FixedEpochGenerator(int days)
        {
            this.seconds = DateTimeOffset.Now.AddDays(days).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FixedEpochGenerator"/>.
        /// </summary>
        /// <param name="seconds">El número de días de se agregarán o restarán a la época. El número puede ser negativo o positivo.</param>
        private FixedEpochGenerator(double seconds)
        {
            this.seconds = seconds;
        }

        /// <summary>
        /// Obtiene el nombre con el que se agrega esta información a la solicitud.
        /// </summary>
        public string Name => "Epoch";

        /// <summary>
        /// Inicializa un un generador de épocas a partir del número de días en <paramref name="days"/> que se agregarán a la fecha actual.
        /// </summary>
        /// <param name="days">El número de días de se agregarán o restarán a la época. El número puede ser negativo o positivo.</param>
        /// <returns>Instancia de <see cref="IEpochGenerator"/>.</returns>
        public static IEpochGenerator FromDatePicker(int days) => new FixedEpochGenerator(days);

        /// <summary>
        /// Inicializa un un generador de épocas a partir de una cantidad de segundos estáticos.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>Instancia de <see cref="IEpochGenerator"/>.</returns>
        public static IEpochGenerator FromStaticSeconds(double seconds) => new FixedEpochGenerator(seconds);

        /// <summary>
        /// Obtiene el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z
        /// </summary>
        /// <returns>
        /// Marca de tiempo Unix, expresado como el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z
        /// </returns>
        public double GetSeconds() => this.seconds;
    }
}