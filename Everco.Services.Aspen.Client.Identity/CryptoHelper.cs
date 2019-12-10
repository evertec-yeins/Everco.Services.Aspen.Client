// -----------------------------------------------------------------------
// <copyright file="CryptoHelper.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-29 08:38 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Expoen métodos que permiten cifrar y descifrar cadenas.
    /// </summary>
    public static class CryptoHelper
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Descifra los datos y devuelve una cadena que contiene los datos descifrados.
        /// </summary>
        /// <param name="encryptedData">Una cadena que contiene datos cifrados.</param>
        /// <param name="entropy">Una matriz de bytes adicional (opcional) que se utilizó para encriptar los datos, o  <see langword="null" /> si no se ha utilizado la matriz de bytes adicional.</param>
        /// <param name="dataProtection">Uno de los valores de la enumeración que especifica el alcance de la protección de datos que se usó para encriptar los datos.</param>
        /// <returns>Una cadena que contiene los datos descifrados.</returns>
        public static string UnprotectString(
            string encryptedData, 
            byte[] entropy = null, 
            DataProtectionScope dataProtection = DataProtectionScope.LocalMachine)
        {
            if (string.IsNullOrWhiteSpace(encryptedData))
            {
                throw new ArgumentNullException(nameof(encryptedData));
            }

            byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, dataProtection);
            return encoding.GetString(decryptedData);
        }

        /// <summary>
        /// Cifra los datos y devuelve una cadena que contiene los datos cifrados.
        /// </summary>
        /// <param name="text">Una cadena que contiene los datos que se desean cifrar.</param>
        /// <param name="entropy">Una matriz de bytes adicional (opcional) que se utilizó para encriptar los datos, o  <see langword="null" /> si no se ha utilizado la matriz de bytes adicional.</param>
        /// <param name="dataProtection">Uno de los valores de la enumeración que especifica el alcance de la protección de datos.</param>
        /// <returns>Una cadena que contiene los datos cifrados.</returns>
        public static string ProtectString(
            string text, 
            byte[] entropy = null,
            DataProtectionScope dataProtection = DataProtectionScope.LocalMachine)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            byte[] encryptedData = ProtectedData.Protect(encoding.GetBytes(text), entropy, dataProtection);
            return Convert.ToBase64String(encryptedData);
        }
    }
}