// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Inquiries.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Internals;
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class DelegatedApp : IInquiriesModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las consultas soportadas de los productos financieros de un usuario.
        /// </summary>
        public IInquiriesModule Inquiries => this;

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas al usuario actual.
        /// </summary>
        /// <returns>
        /// Lista de tipo <see cref="IAccountInfo" /> con la información de las cuentas del usuario actual.
        /// </returns>
        public IList<AccountExtendedInfo> GetAccounts()
        {
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.AccountsFromCurrentUser);
            return this.Execute<List<AccountExtendedInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas al usuario actual.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<AccountExtendedInfo>> GetAccountsAsync()
        {
            return await Task.Run(this.GetAccounts);
        }

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">Identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de tipo <see cref="IBalanceInfo" /> con la información de saldos de las cuentas del usuario actual.
        /// </returns>
        public IList<BalanceExtendedInfo> GetBalances(string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters().AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.BalancesFromCurrentUser, endpointParameters);
            return this.Execute<List<BalanceExtendedInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<BalanceExtendedInfo>> GetBalancesAsync(string accountId)
        {
            return await Task.Run(() => this.GetBalances(accountId));
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada del usuario actual.
        /// </returns>
        public IList<MiniStatementInfo> GetStatements(string accountId, string accountTypeId = null)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.StatementsFromCurrentUser, endpointParameters);
            return this.Execute<List<MiniStatementInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada del usuario actual.
        /// </returns>
        public async Task<IList<MiniStatementInfo>> GetStatementsAsync(string accountId, string accountTypeId = null)
        {
            return await Task.Run(() => this.GetStatements(accountId, accountTypeId));
        }
    }
}
