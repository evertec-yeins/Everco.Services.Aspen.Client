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
    using System.Threading.Tasks;
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
        /// Obtiene la información de las cuentas vinculadas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<TransferAccountInfo>> GetTransferAccountsAsync()
        {
            return await Task.Run(this.GetTransferAccounts);
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="pinNumber">El pin transaccional del usuario autenticado.</param>
        /// <param name="accountNumber">El número de cuenta que se desea registrar o <c>null</c> para que el sistema busque el número de cuenta asociado con el tarjetahabiente.</param>
        /// <param name="alias">El nombre con el que se desea identificar la cuenta a registrar.</param>
        public void LinkTransferAccount(
            string cardHolderDocType,
            string cardHolderDocNumber,
            string pinNumber,
            string accountNumber = null,
            string alias = null)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(cardHolderDocType, nameof(cardHolderDocType));
                Throw.IfNullOrEmpty(cardHolderDocNumber, nameof(cardHolderDocNumber));
                Throw.IfNullOrEmpty(alias, nameof(alias));
            }

            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                cardHolderDocType,
                cardHolderDocNumber,
                alias,
                accountNumber,
                pinNumber);
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.LinkTransferAccountFromCurrentUser);
            request.AddJsonBody(linkTransferAccountInfo);
            this.Execute(request);
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="pinNumber">El pin transaccional del usuario autenticado.</param>
        /// <param name="accountNumber">El número de cuenta que se desea registrar o <c>null</c> para que el sistema busque el número de cuenta asociado con el tarjetahabiente.</param>
        /// <param name="alias">El nombre con el que se desea identificar la cuenta a registrar.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task LinkTransferAccountAsync(
            string cardHolderDocType,
            string cardHolderDocNumber,
            string pinNumber,
            string accountNumber = null,
            string alias = null)
        {
            await Task.Run(() => this.LinkTransferAccount(
                cardHolderDocType,
                cardHolderDocNumber,
                pinNumber,
                accountNumber,
                alias));
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        public void LinkTransferAccount(ILinkTransferAccountInfo accountInfo)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNull(accountInfo, nameof(accountInfo));
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.LinkTransferAccountFromCurrentUser);
            request.AddJsonBody(accountInfo);
            this.Execute(request);
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task LinkTransferAccountAsync(ILinkTransferAccountInfo accountInfo)
        {
            await Task.Run(() => this.LinkTransferAccount(accountInfo));
        }

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas para transferencia de saldos del usuario actual.
        /// </summary>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        public void UnlinkTransferAccount(string alias)
        {
            if (!ServiceLocator.Instance.Runtime.IsDevelopment)
            {
                Throw.IfNullOrEmpty(alias, nameof(alias));
            }

            EndpointParameters endpointParameters = new EndpointParameters
                                                        {
                                                            { "@[Alias]", alias }
                                                        };
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.UnlinkTransferAccountFromCurrentUserByAlias, endpointParameters);
            this.Execute(request);
        }

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas para transferencia de saldos del usuario actual.
        /// </summary>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task UnlinkTransferAccountAsync(string alias)
        {
            await Task.Run(() => this.UnlinkTransferAccount(alias));
        }
    }
}