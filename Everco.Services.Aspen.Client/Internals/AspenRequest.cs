// -----------------------------------------------------------------------
// <copyright file="AspenRequest.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using RestSharp;

    /// <summary>
    /// Representa la información de una solicitud HTTP al servicio Aspen.
    /// </summary>
    internal class AspenRequest : RestRequest
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AspenRequest" />.
        /// </summary>
        /// <param name="resource">Url del recurso solicitado.</param>
        /// <param name="method">Método o verbo HTTP para invocar el recurso.</param>
        /// <param name="accept">Texto que se envía en la cabecera Accept de la solicitud.</param>
        /// <param name="contentType">Texto que se envía en la cabecera Content-Type de la solicitud.</param>
        /// <param name="dataFormat">Formato de los datos que se envian con la solicitud.</param>
        internal AspenRequest(
            string resource,
            Method method,
            string accept = "application/json",
            string contentType = "application/json; charset=utf-8",
            DataFormat dataFormat = DataFormat.Json) : base(resource, method, dataFormat)
        {
            Throw.IfNullOrEmpty(resource, nameof(resource));
            if (!string.IsNullOrWhiteSpace(accept))
            {
                this.AddHeader("Accept", accept);
            }

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                this.AddHeader("Content-Type", contentType);
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AspenRequest" />.
        /// </summary>
        /// <param name="scope">Alcance de la aplicación que solicita la información.</param>
        /// <param name="mappingInfo">Valor de la enumeración de donde se extraen la Url y el método.</param>
        /// <param name="accept">Texto que se envía en la cabecera Accept de la solicitud.</param>
        /// <param name="contentType">Texto que se envía en la cabecera Content-Type de la solicitud.</param>
        /// <param name="dataFormat">Formato de los datos que se envian con la solicitud.</param>
        internal AspenRequest(
            Scope scope,
            EndpointMapping mappingInfo,
            string accept = "application/json",
            string contentType = "application/json; charset=utf-8",
            DataFormat dataFormat = DataFormat.Json) : this(
                mappingInfo.GetEndPointMappingInfo(scope).Resource,
                mappingInfo.GetEndPointMappingInfo(scope).Method,
                accept,
                contentType,
                dataFormat)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AspenRequest"/>
        /// </summary>
        /// <param name="scope">Alcance de la aplicación que solicita la información.</param>
        /// <param name="mappingInfo">Valor de la enumeración de donde se extraen la Url y el método.</param>
        internal AspenRequest(Scope scope, EndpointMapping mappingInfo) : this(
                mappingInfo.GetEndPointMappingInfo(scope).Resource,
                mappingInfo.GetEndPointMappingInfo(scope).Method)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AspenRequest"/>
        /// </summary>
        /// <param name="scope">Alcance de la aplicación que solicita la información.</param>
        /// <param name="mappingInfo">Valor de la enumeración de donde se extraen la Url y el método.</param>
        /// <param name="endpointParameters">La colección de parámetros para el endpoint.</param>
        internal AspenRequest(Scope scope, EndpointMapping mappingInfo, EndpointParameters endpointParameters) : this(
            new PlaceholderFormatter(mappingInfo.GetEndPointMappingInfo(scope).Resource, endpointParameters).ToString(), 
            mappingInfo.GetEndPointMappingInfo(scope).Method)
        {
        }
    }
}