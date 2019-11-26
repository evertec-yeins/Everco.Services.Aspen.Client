// -----------------------------------------------------------------------
// <copyright file="SecureIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 12:53 PM</date>
// ----------------------------------------------------------------------
// ReSharper disable MemberCanBePrivate.Global
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Obtiene la información que se utiliza para autenticar la solicitud en el servicio Aspen de un archivo de texto cifrado.
    /// </summary>
    public class SecureIdentity : IAppIdentity
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly byte[] entropy;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly DataProtectionScope protectionScope;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private string lastContent;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="entropy">Una matriz de bytes (opcional) que se utiliza para aumentar la complejidad del cifrado, o  <see langword="null" /> para no agregar complejidad adicional.</param>
        public SecureIdentity(
            byte[] entropy = null) : this(entropy, Encoding.UTF8, DataProtectionScope.LocalMachine)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="entropy">Una matriz de bytes (opcional) que se utiliza para aumentar la complejidad del cifrado, o  <see langword="null" /> para no agregar complejidad adicional.</param>
        /// <param name="encoding">Codificación que se desea utilizar en el proceso de cifrado.</param>
        public SecureIdentity(
            byte[] entropy,
            Encoding encoding) : this(entropy, encoding, DataProtectionScope.LocalMachine)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="entropy">Una matriz de bytes (opcional) que se utiliza para aumentar la complejidad del cifrado, o  <see langword="null" /> para no agregar complejidad adicional.</param>
        /// <param name="encoding">Codificación que se desea utilizar en el proceso de cifrado.</param>
        /// <param name="protectionScope">El alcance del cifrado.</param>
        public SecureIdentity(
            byte[] entropy,
            Encoding encoding,
            DataProtectionScope protectionScope)
        {
            this.entropy = entropy;
            this.encoding = encoding;
            this.protectionScope = protectionScope;
        }

        /// <summary>
        /// Cifra los datos y devuelve una cadena que contiene los datos cifrados.
        /// </summary>
        /// <param name="input">Texto en claro que se desea cifrar.</param>
        /// <returns>Cadena que contiene los datos cifrados.</returns>
        public string Encrypt(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if (string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new ArgumentNullException(nameof(apiSecret));
            }

            string input = $"{apiKey}|{apiSecret}";
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            byte[] userData = this.encoding.GetBytes(input);
            byte[] encryptedData = ProtectedData.Protect(userData, this.entropy, this.protectionScope);
            this.lastContent = Convert.ToBase64String(encryptedData);
            return this.lastContent;
        }

        /// <summary>
        /// Descifra los datos y devuelve una cadena que contiene los datos descifrados.
        /// </summary>
        /// <param name="encryptedData">Datos cifrados que se desean descifrar.</param>
        /// <returns>Cadena que contiene los datos descifrados.</returns>
        public string Decrypt(string encryptedData)
        {
            byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), this.entropy, this.protectionScope);
            return this.encoding.GetString(decryptedData);
        }

        public void SaveTo(string path, bool @override = true)
        {
            if (File.Exists(path))
            {
                if (!@override)
                {
                    throw new IOException($" The file {path} already exists.");
                }
            }

            List<object> graph = new List<object>
            {
                this.encoding, 
                this.entropy, 
                this.encoding.GetBytes(this.lastContent),
                this.protectionScope
            };

            using FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fs, graph);
        }

        public static SecureIdentity FromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            using FileStream reader = new FileStream(path, FileMode.Open);
            BinaryFormatter deserializer = new BinaryFormatter();
            List<object> result = (List<object>)deserializer.Deserialize(reader);
            Encoding encoding = (Encoding) result[0];
            byte[] entropy = (byte[])result[1];
            byte[] content = (byte[])result[2];
            DataProtectionScope protectionScope = (DataProtectionScope)result[3];

            SecureIdentity secureIdentity = new SecureIdentity(entropy, encoding, protectionScope)
            {
                ApiKey = "", 
                ApiSecret = ""
            };

            return secureIdentity;
        }

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret { get; private set; }
    }
}