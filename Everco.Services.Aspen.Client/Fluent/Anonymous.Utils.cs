// -----------------------------------------------------------------------
// <copyright file="Anonymous.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Threading.Tasks;
    using Everco.Services.Aspen.Client.Internals;
    using Everco.Services.Aspen.Client.Modules.Anonymous;
    using Identity;
    using RestSharp;

    /// <summary>
    /// Expone las operaciones que permite conectar con las operaciones disponibles en el servicio Aspen que no requieren firma.
    /// </summary>
    public sealed partial class Anonymous : IUtilsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public IUtilsModule Utils => this;

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

        /// <summary>
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación que generó el error.</param>
        /// <param name="username">El identificador del último usuario que uso la aplicación antes de generarse el error.</param>
        /// <param name="errorReport">La información del reporte de error generado en la aplicación.</param>
        /// <returns>
        /// Instancia de <see cref="Task" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task SaveAppCrashAsync(string apiKey, string username, string errorReport)
        {
            await Task.Run(() => this.SaveAppCrash(apiKey, username, errorReport));
        }
    }
}