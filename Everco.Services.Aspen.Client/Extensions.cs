// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client
{
    /// <summary>
    /// Expone métdos de extensión para varios tipos.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Retorna <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío.
        /// </summary>
        /// <param name="input">Texto a evaluar.</param>
        /// <param name="defaultValue">Texto de reemplazo o  <see langword="null" /> para utilizar el texto predeterminado.</param>
        /// <returns>
        /// <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío; de lo contrario<paramref name="input" />.
        /// </returns>
        internal static string DefaultIfNullOrEmpty(this string input, string defaultValue = null) => string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }
}