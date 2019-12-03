// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Management.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Everco.Services.Aspen.Client.Internals;
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone las operaciones soportadas para acceder a entidades de administración para una aplicación con alcance de delegada.
    /// </summary>
    public sealed partial class DelegatedApp : IManagementModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones de administración varias.
        /// </summary>
        public IManagementModule Management => this;

        /// <summary>
        /// Obtiene la información de las cuentas vinculadas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <returns>
        /// Lista de instancias de <see cref="ITransferAccountInfo" /> con la información de las cuentas vinculadas al usuario actual.
        /// </returns>
        public IList<TransferAccountInfo> GetTransferAccounts()
        {
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.TransferAccountsFromCurrentUser);
            return this.Execute<List<TransferAccountInfo>>(request) ?? Enumerable.Empty<TransferAccountInfo>().ToList();
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        public void LinkTransferAccount(ILinkTransferAccountPinSignedInfo accountInfo)
        {
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.LinkTransferAccountFromCurrentUser);
            request.AddJsonBody(accountInfo);
            this.Execute(request);
        }

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas para transferencia de saldos del usuario actual.
        /// </summary>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        public void UnlinkTransferAccount(string alias)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddAccountAlias(alias);
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.UnlinkTransferAccountFromCurrentUserByAlias, endpointParameters);
            this.Execute(request);
        }
    }
}
