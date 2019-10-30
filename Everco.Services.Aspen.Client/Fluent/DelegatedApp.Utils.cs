// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using Entities;
    using Everco.Services.Aspen.Client.Auth;
    using Internals;
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class DelegatedApp : IUtilsModule
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

            IRestRequest request = new AspenRequest(Scope.Anonymous, EndpointMapping.Encrypt);
            request.AddParameter("Input", value);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            IRestResponse response = this.RestClient.Execute(request);

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

        /// <summary>
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="errorReport">Información del reporte de error generado en la aplicación.</param>
        /// <param name="userName">Información del último usuario que uso la aplicación antes de generarse el error.</param>
        public void SaveAppCrash(string errorReport, string userName)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(errorReport, "errorReport");
                Throw.IfNullOrEmpty(userName, "userName");
            }

            IRestRequest request = new AspenRequest(Scope.Anonymous, EndpointMapping.AppCrash);
            request.AddParameter("ErrorReport", errorReport);
            request.AddParameter("Username", userName);
            IDeviceInfo deviceInfo = CacheStore.GetDeviceInfo() ?? new DeviceInfo();
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }
        }
    }
}
