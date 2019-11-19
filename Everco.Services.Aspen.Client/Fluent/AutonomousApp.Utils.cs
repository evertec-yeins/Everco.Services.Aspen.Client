// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Net;
    using Entities;
    using Internals;
    using Modules.Autonomous;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : IUtilsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public IUtilsModule Utils => this;

        /// <summary>
        /// Cifra una cadena de texto usando el algoritmo de cifrado del servicio.
        /// </summary>
        /// <param name="value">La cadena de texto en claro a cifrar.</param>
        /// <returns>
        /// Cadena cifrada.
        /// </returns>
        public string Encrypt(string value)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(value, "value");
            }

            IRestRequest request = new AspenRequest(
                Scope.Anonymous,
                EndpointMapping.Encrypt,
                contentType: "application/x-www-form-urlencoded");
            request.AddParameter("Input", value);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Resource => {this.RestClient.BaseUrl}{request.Resource}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Method => {request.Method}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Proxy => {(ServiceLocator.Instance.WebProxy as WebProxy)?.Address?.ToString() ?? "NONSET"}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Headers => {JsonConvert.SerializeObject(this.GetHeaders(request.Parameters))}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Body => {JsonConvert.SerializeObject(this.GetBody(request.Parameters))}");
            IRestResponse response = this.RestClient.Execute(request);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusCode => {(int)response.StatusCode} ({response.StatusCode.ToString()})");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"StatusDescription => {response.StatusDescription}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseStatus => {response.ResponseStatus}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"ResponseContent => {response.Content}");

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response.Content;
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento predeterminados.
        /// </returns>
        public IList<DocTypeInfo> GetDefaultDocTypes()
        {
            IRestRequest request = new AspenRequest(Scope.Anonymous, EndpointMapping.DefaultDocTypes);
            return this.Execute<List<DocTypeInfo>>(request);
        }
    }
}
