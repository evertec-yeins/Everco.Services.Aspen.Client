// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Settings.cs" company="Evertec Colombia">
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
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance delegada.
    /// </summary>
    public sealed partial class DelegatedApp : ISettingsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public ISettingsModule Settings => this;

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento soportados.
        /// </returns>
        public IList<DocTypeInfo> GetDocTypes()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.DocTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<DocTypeInfo>>(request);
        }
    }
}
