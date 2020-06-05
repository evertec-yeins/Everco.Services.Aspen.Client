﻿// -----------------------------------------------------------------------
// <copyright file="Anonymous.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Everco.Services.Aspen.Client.Internals;
    using Everco.Services.Aspen.Client.Modules.Anonymous;
    using Everco.Services.Aspen.Entities;
    using RestSharp;

    /// <summary>
    /// Expone las operaciones que permite conectar con las operaciones disponibles en el servicio Aspen que no requieren firma.
    /// </summary>
    public sealed partial class Anonymous : ISettingsModule
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string CacheKeyAppSettings = "AppMovSettings";

        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización de la aplicación en el sistema Aspen.
        /// </summary>
        public ISettingsModule Settings => this;

        /// <summary>
        /// Obtiene la configuración de valores de configuración soportados para la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <returns>
        /// Colección de clavesy valores soportados para la aplicación.
        /// </returns>
        public AppMovSettings GetAppSettings(string apiKey)
        {
            AppMovSettings appMovSettings = this.cache.Get<AppMovSettings>(CacheKeyAppSettings);

            if (appMovSettings != null)
            {
                ServiceLocator.Instance.LoggingProvider.WriteDebug("Anonymous => Return App settings from cache.");
                return appMovSettings;
            }

            IRestRequest request = new AspenRequest(Scope.Anonymous, EndpointMapping.AppMovSettings);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, apiKey);
            appMovSettings = this.Execute<AppMovSettings>(request);
            this.cache.Add(CacheKeyAppSettings, appMovSettings, this.cacheOptions);
            ServiceLocator.Instance.LoggingProvider.WriteDebug("Anonymous => App settings saved to cache.");
            return appMovSettings;
        }

        /// <summary>
        /// Obtiene la configuración de valores de configuración soportados para la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<AppMovSettings> GetAppSettingsAsync(string apiKey)
        {
            return await Task.Run(() => this.GetAppSettings(apiKey));
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
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<DocTypeInfo>> GetDefaultDocTypesAsync()
        {
            return await Task.Run(this.GetDefaultDocTypes);
        }
    }
}