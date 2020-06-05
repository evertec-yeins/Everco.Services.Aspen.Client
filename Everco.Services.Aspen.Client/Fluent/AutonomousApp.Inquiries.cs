// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Inquiries.cs" company="Evertec Colombia">
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
    using Everco.Services.Aspen.Client.Internals;
    using RestSharp;
    using V10 = Modules.Autonomous.V10;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : V10.IInquiriesModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a operaciones de consulta de productos financieros.
        /// </summary>
        public V10.IInquiriesModule Inquiries => this;

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IAccountInfo"/> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        public IList<AccountInfo> GetAccounts(string docType, string docNumber)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByUserIdentity, endpointParameters);
            return this.Execute<List<AccountInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<AccountInfo>> GetAccountsAsync(string docType, string docNumber)
        {
            return await Task.Run(() => this.GetAccounts(docType, docNumber));
        }

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IAccountInfo"/> con la información de las cuentas del usuario a partir del alias especificado.
        /// </returns>
        public IList<AccountInfo> GetAccountsByAlias(string channelId, string enrollmentAlias)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByAlias, endpointParameters);
            return this.Execute<List<AccountInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<AccountInfo>> GetAccountsByAliasAsync(string channelId, string enrollmentAlias)
        {
            return await Task.Run(() => this.GetAccountsByAlias(channelId, enrollmentAlias));
        }

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario especificado.
        /// </returns> 
        public IList<BalanceInfo> GetBalances(
            string docType,
            string docNumber,
            string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByUserIdentity, endpointParameters);
            return this.Execute<List<BalanceInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<BalanceInfo>> GetBalancesAsync(
            string docType,
            string docNumber,
            string accountId)
        {
            return await Task.Run(() => this.GetBalances(
                docType,
                docNumber,
                accountId));
        }

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario a partir del alias especificado.
        /// </returns>
        public IList<BalanceInfo> GetBalancesByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByAlias, endpointParameters);
            return this.Execute<List<BalanceInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<BalanceInfo>> GetBalancesByAliasAsync(
            string channelId,
            string enrollmentAlias,
            string accountId)
        {
            return await Task.Run(() => this.GetBalancesByAlias(
                channelId,
                enrollmentAlias,
                accountId));
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IMiniStatementInfo"/> con la información de los movimientos financieros de la cuenta asociada al usuario especificado.
        /// </returns>
        public IList<MiniStatementInfo> GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByUserIdentity, endpointParameters);
            return this.Execute<List<MiniStatementInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<MiniStatementInfo>> GetStatementsAsync(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null)
        {
            return await Task.Run(() => this.GetStatements(
                docType,
                docNumber,
                accountId,
                accountTypeId));
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IMiniStatementInfo"/> con la información de los movimientos financieros de la cuenta asociada al usuario a partir del alias especificado.
        /// </returns>
        public IList<MiniStatementInfo> GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByAlias, endpointParameters);
            return this.Execute<List<MiniStatementInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<MiniStatementInfo>> GetStatementsByAliasAsync(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null)
        {
            return await Task.Run(() => this.GetStatementsByAlias(
                channelId,
                enrollmentAlias,
                accountId,
                accountTypeId));
        }
    }
}