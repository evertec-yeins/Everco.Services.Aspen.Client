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
    using Entities;
    using Internals;
    using Modules.Autonomous;
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
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento predeterminados.
        /// </returns>
        public IList<DocTypeInfo> GetDefaultDocTypes()
        {
            string resource = Routes.Utils.DocTypes;
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<DocTypeInfo>>(request);
        }

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

            string resource = Routes.Utils.Crypto;
            IRestRequest request = new AspenRequest(resource, Method.POST);
            request.AddParameter("Input", value);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response.Content;
        }
    }
}
