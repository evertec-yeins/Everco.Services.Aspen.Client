// -----------------------------------------------------------------------
// <copyright file="PlaceholderFormatter.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 17:25 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Expone operaciones para el reemplazo de marcadores de posición nombrados en un <see cref="string"/>.
    /// </summary>
    internal class PlaceholderFormatter
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly string format;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly IDictionary<string, object> placeholders;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PlaceholderFormatter"/>
        /// </summary>
        /// <param name="format">Cadena de formato con los marcadores nombrados para reemplazar.</param>
        public PlaceholderFormatter(string format)
        {
            Throw.IfNullOrEmpty(format, nameof(format));
            this.format = format;
            this.placeholders = new Dictionary<string, object>();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PlaceholderFormatter"/>
        /// </summary>
        /// <param name="format">Cadena de formato con los marcadores nombrados para reemplazar.</param>
        /// <param name="placeholders">Una colección de clave y valores para inicializar los marcadores de posición.</param>
        public PlaceholderFormatter(string format, IDictionary<string, object> placeholders) : this(format)
        {
            this.placeholders = placeholders ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Agrega el valor del marcador a la lista de valores para reemplazar.
        /// </summary>
        /// <param name="key">Texto del marcador.</param>
        /// <param name="value">Valor del marcador.</param>
        public void Add(string key, object value)
        {
            Throw.IfNullOrEmpty(key, nameof(key));
            Throw.IfNull(value, nameof(value));
            this.placeholders.Add(key, value);
        }

        /// <summary>
        /// Retorna una instancia de <see cref="System.String" /> que representa el valor de la instacia actual.
        /// </summary>
        /// <returns>Un <see cref="System.String" /> que representa esta instancia.</returns>
        public override string ToString()
        {
            return this.placeholders.Aggregate(
                this.format,
                (current, parameter) => current.Replace(parameter.Key, parameter.Value.ToString()));
        }
    }
}