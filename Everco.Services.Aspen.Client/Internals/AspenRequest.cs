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
        /// <param name="resource">El recurso solicitado.</param>
        /// <param name="method">Método o verbo HTTP para invocar el recurso.</param>
        internal AspenRequest(string resource, Method method) :
            base(resource, method, DataFormat.Json)
        {
            Throw.IfNullOrEmpty(resource, nameof(resource));
            //this.AddHeader("Accept", "application/json");
            //this.AddHeader("Content-Type", "application/json; charset=utf-8");
        }
    }
}