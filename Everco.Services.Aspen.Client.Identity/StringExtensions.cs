// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-29 09:06 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;

    /// <summary>
    /// Expone métodos de extensión para la clase <see cref="String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Intenta descifrar los datos y devuelve una cadena que contiene los datos descifrados o <paramref name="text"/> si los datos no se pueden descifrar.
        /// </summary>
        /// <param name="text">Texto que se intenta descrifrar.</param>
        /// <param name="entropy">Una matriz de bytes adicional (opcional) que se utilizó para encriptar los datos, o  <see langword="null" /> si no se ha utilizado la matriz de bytes adicional.</param>
        /// <returns>Una cadena que contiene los datos descifrados o <paramref name="text"/> si los datos no se pueden descifrar.</returns>
        public static string TryDecrypt(this string text, byte[] entropy = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            try
            {
                return CryptoHelper.UnprotectString(text, entropy);
            }
            catch (SystemException)
            {
                // FormatException, CryptographicException //
                return text;
            }
        }
    }
}