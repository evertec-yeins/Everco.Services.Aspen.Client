// -----------------------------------------------------------------------
// <copyright file="Printer.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 17:40 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Un utilitario para imprimir contenido en la consola.
    /// </summary>
    public static class Printer
    {
        /// <summary>
        /// Imprime el valor especificado.
        /// </summary>
        /// <param name="value">El valor a imprimir</param>
        /// <param name="title">Un título opcional.</param>
        public static void Print(object value, string title = null)
        {
            Console.WriteLine(Environment.NewLine);

            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title.PadBoth(50));
            }

            if (value == null)
            {
                return;
            }

            string jsonValue = JsonConvert.SerializeObject(value, Formatting.Indented, new StringEnumConverter());
            Console.WriteLine(jsonValue);
        }

        /// <summary>
        /// Rellena tanto a la izquierda como a la derecha con el caracter especificado en <paramref name="paddingChar"/>
        /// hasta completar una cadena de la longitud especificada en <paramref name="length"/>.
        /// </summary>
        /// <param name="text">Texto que se va a rellenar.</param>
        /// <param name="length">Largo del texto de salida.</param>
        /// <param name="paddingChar">Caracter que se desea utilizar para el relleno.</param>
        /// <returns>
        /// Cadena rellena tanto a la izquierda como a la derecha con el caracter
        /// especificado en <paramref name="paddingChar"/>.
        /// </returns>
        private static string PadBoth(this string text, int length, char paddingChar = '=')
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            int textLength = length - text.Length;
            int width = (textLength / 2) + text.Length;
            return text.PadLeft(width, paddingChar).PadRight(length, paddingChar);
        }
    }
}