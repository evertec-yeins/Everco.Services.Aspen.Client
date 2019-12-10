// -----------------------------------------------------------------------
// <copyright file="JsonNetSerializer.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-03 14:24 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using Newtonsoft.Json;
    using RestSharp;
    using RestSharp.Serialization;

    /// <summary>
    /// Implementa un serializador personalizado para RestSharp basado en Json.NET
    /// </summary>
    /// <seealso cref="IRestSerializer" />
    internal class JsonNetSerializer : IRestSerializer
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="JsonNetSerializer" />.
        /// </summary>
        public JsonNetSerializer()
        {
            this.DataFormat = DataFormat.Json;
            this.ContentType = "application/json";
            this.SupportedContentTypes = new[]
                                             {
                                                 "application/json",
                                                 "text/json",
                                                 "text/x-json",
                                                 "text/javascript",
                                                 "*+json"
                                             };
        }

        /// <summary>
        /// Obtiene una instancia predeterminada del serializador.
        /// </summary>
        public static IRestSerializer Default => new JsonNetSerializer();

        /// <summary>
        /// Obtiene o establece el tipo de contenido.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Obtiene el formato de datos.
        /// </summary>
        public DataFormat DataFormat { get; }

        /// <summary>
        /// Obtiene los tipos de contenido soportados.
        /// </summary>
        public string[] SupportedContentTypes { get; }

        /// <summary>
        /// Deserializa la respuesta especificada.
        /// </summary>
        /// <typeparam name="T">El tipo al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="response">La respuesta generada por el servicio.</param>
        /// <returns>Instancia de <typeparamref name="T"/> con la información de respuesta del servicio .</returns>
        public T Deserialize<T>(IRestResponse response) => JsonConvert.DeserializeObject<T>(response.Content);

        /// <summary>
        /// Serializa el objeto especificado.
        /// </summary>
        /// <param name="obj">El objeto de entrada.</param>
        /// <returns>Cadena de texto en formato json que representa la serialización de <paramref name="obj"/>.</returns>
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);

        /// <summary>
        /// Serializa el parámetro especificado.
        /// </summary>
        /// <param name="parameter">El parámetro de entrada.</param>
        /// <returns>Cadena de texto en formato json que representa la serialización de <paramref name="parameter"/>.</returns>
        public string Serialize(Parameter parameter) => JsonConvert.SerializeObject(parameter.Value);
    }
}