// -----------------------------------------------------------------------
// <copyright file="Anonymous.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using Everco.Services.Aspen.Client.Identity;
    using Everco.Services.Aspen.Client.Internals;
    using Everco.Services.Aspen.Client.Modules.Anonymous;
    using Everco.Services.Aspen.Entities;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class Anonymous : IUtilsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a operaciones para utilidades varias.
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
            IRestRequest request = new AspenRequest(Scope.Anonymous, EndpointMapping.DefaultDocTypes);
            return this.Execute<List<DocTypeInfo>>(request);
        }

        /// <summary>
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación que generó el error.</param>
        /// <param name="username">El identificador del último usuario que uso la aplicación antes de generarse el error.</param>
        /// <param name="errorReport">La información del reporte de error generado en la aplicación.</param>
        public void SaveAppCrash(string apiKey, string username, string errorReport)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(apiKey, nameof(apiKey));
                Throw.IfNullOrEmpty(errorReport, nameof(errorReport));
                Throw.IfNullOrEmpty(username, nameof(username));
            }

            IRestRequest request = new AspenRequest(
                Scope.Anonymous,
                EndpointMapping.AppCrash,
                contentType: "application/x-www-form-urlencoded");
            request.AddParameter("ErrorReport", errorReport);
            request.AddParameter("Username", username);
            IDeviceInfo deviceInfo = CacheStore.GetDeviceInfo() ?? DeviceInfo.Current;
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, apiKey);
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());
            this.Execute(request);
        }
    }
}