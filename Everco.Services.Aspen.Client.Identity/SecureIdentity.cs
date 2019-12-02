// -----------------------------------------------------------------------
// <copyright file="SecureIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 12:53 PM</date>
// ----------------------------------------------------------------------
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToUsingDeclaration
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    /// <summary>
    /// Obtiene la información que se utiliza para autenticar la solicitud en el servicio Aspen de un archivo de texto cifrado.
    /// </summary>
    [Serializable]
    public class SecureIdentity : IAppIdentity, ISecureStorage
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly byte[] entropy;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="apiKey">Valor del ApiKey que se desea cifrar.</param>
        /// <param name="apiSecret">Valor del ApiSecret que se desea cifrar.</param>
        public SecureIdentity(string apiKey, string apiSecret) :
            this(apiKey, apiSecret, null, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="entropy">Una matriz de bytes (opcional) que se utiliza para aumentar la complejidad del cifrado, o  <see langword="null" /> para no agregar complejidad adicional.</param>
        /// <param name="encoding">Codificación que se desea utilizar en el proceso de cifrado.</param>
        public SecureIdentity(
            byte[] entropy,
            Encoding encoding)
        {
            this.entropy = entropy;
            this.encoding = encoding;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        /// <param name="apiKey">Valor del ApiKey que se desea cifrar.</param>
        /// <param name="apiSecret">Valor del ApiSecret que se desea cifrar.</param>
        /// <param name="entropy">Una matriz de bytes (opcional) que se utiliza para aumentar la complejidad del cifrado, o  <see langword="null" /> para no agregar complejidad adicional.</param>
        /// <param name="encoding">Codificación que se desea utilizar en el proceso de cifrado.</param>
        public SecureIdentity(
            string apiKey,
            string apiSecret,
            byte[] entropy,
            Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if (string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new ArgumentNullException(nameof(apiSecret));
            }

            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.entropy = entropy;
            this.encoding = encoding;
        }

        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="SecureIdentity"/>
        /// </summary>
        private SecureIdentity()
        {
        }
        
        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret { get; private set; }

        /// <summary>
        /// Crea una instancia de <see cref="SecureIdentity"/> a partir del archivo especificado.
        /// </summary>
        /// <param name="path">Ruta del archivo con la información cifrada.</param>
        /// <returns>Instancia de <see cref="SecureIdentity"/> con la información a partir del archivo.</returns>
        public static SecureIdentity FromFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException();
                }

                using (FileStream reader = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    List<KeyValuePair<string, object>> result = (List<KeyValuePair<string, object>>)deserializer.Deserialize(reader);
                    byte[] entropy = (byte[])result.Find(m => m.Key == Keys.Entropy).Value;
                    Encoding encoding = Encoding.GetEncoding((int)result.Find(m => m.Key == Keys.Encoding).Value);
                    string apiKey = (string)result.Find(m => m.Key == Keys.ApiKey).Value;
                    string apiSecret = (string)result.Find(m => m.Key == Keys.ApiSecret).Value;
                    
                    SecureIdentity secureIdentity = new SecureIdentity(entropy, encoding)
                    {
                        ApiKey = CryptoHelper.UnprotectString(apiKey, entropy),
                        ApiSecret = CryptoHelper.UnprotectString(apiSecret, entropy)
                    };
                    
                    return secureIdentity;
                }
            }
            catch (Exception exception)
            {
                throw new IdentityException(exception);
            }
        }

        /// <summary>
        /// Cifra los datos y devuelve un objeto que permite guardar los datos cifrados.
        /// </summary>
        /// <returns>Instancia de <see cref="ISecureStorage"/> que le permite guardar la información en un archivo.</returns>
        public ISecureStorage Encrypt()
        {
            this.ApiKey = CryptoHelper.ProtectString(this.ApiKey, this.entropy);
            this.ApiSecret = CryptoHelper.ProtectString(this.ApiSecret, this.entropy);
            return this;
        }

        /// <summary>
        /// Guarda en un archivo la información.
        /// </summary>
        /// <param name="path">Ruta de acceso del archivo donde se guarda la información.</param>
        /// <exception cref="IOException">The file {path} already exists.</exception>
        public void SaveTo(string path)
        {
            if (File.Exists(path))
            {
                throw new IOException($" The file {path} already exists.");
            }

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                List<KeyValuePair<string, object>> graph = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(Keys.ApiKey, this.ApiKey),
                    new KeyValuePair<string, object>(Keys.ApiSecret, this.ApiSecret),
                    new KeyValuePair<string, object>(Keys.Entropy, this.entropy),
                    new KeyValuePair<string, object>(Keys.Encoding, this.encoding.CodePage)
                };
                formatter.Serialize(stream, graph);
            }
        }

        /// <summary>
        /// Define las claves que se utilizan en el proceso de seruialización.
        /// </summary>
        private static class Keys
        {
            /// <summary>
            /// Para uso interno.
            /// </summary>
            internal const string ApiKey = "b92e";

            /// <summary>
            /// Para uso interno.
            /// </summary>
            internal const string ApiSecret = "b8080a34";

            /// <summary>
            /// Para uso interno.
            /// </summary>
            internal const string Encoding = "1949:4574";

            /// <summary>
            /// Para uso interno.
            /// </summary>
            internal const string Entropy = "3ec6b4f3acd1";
        }
    }
}