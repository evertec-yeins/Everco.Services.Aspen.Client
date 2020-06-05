// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Inquiries{v1.1}.cs" company="Evertec Colombia">
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
    using Everco.Services.Aspen.Client.Modules.Autonomous;
    using RestSharp;
    using V11 = Modules.Autonomous.V11;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : V11.IInquiriesModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a operaciones de consulta de productos financieros.
        /// </summary>
        public V11.IInquiriesModule InquiriesV11 => this;

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de las cuentas asociadas a un usuario de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{IAccountInfo}" /> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<AccountResultInfo> IAccountInquiries<AccountResultInfo>.GetAccounts(
            string docType,
            string docNumber)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByUserIdentity, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<AccountResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de las cuentas asociadas a un usuario de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<AccountResultInfo>> IAccountInquiries<AccountResultInfo>.GetAccountsAsync(
            string docType,
            string docNumber)
        {
            return await Task.Run(() => this.InquiriesV11.GetAccounts(docType, docNumber));
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula el resumen de las cuentas asociadas a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{IAccountInfo}" /> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<AccountResultInfo> IAccountInquiries<AccountResultInfo>.GetAccountsByAlias(
            string channelId,
            string enrollmentAlias)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByAlias, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<AccountResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula el resumen de las cuentas asociadas a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<AccountResultInfo>> IAccountInquiries<AccountResultInfo>.GetAccountsByAliasAsync(
            string channelId,
            string enrollmentAlias)
        {
            return await Task.Run(() => this.InquiriesV11.GetAccountsByAlias(channelId, enrollmentAlias));
        }

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de los saldos de una cuenta asociada a un usuario de forma segura.B
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{IBalanceInfo}" /> con la información del resultado por la solicitud de los saldos de una cuenta del usuario especificado.
        /// </returns>
        IList<BalanceResultInfo> IBalanceInquiries<BalanceResultInfo>.GetBalances(
            string docType,
            string docNumber,
            string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByUserIdentity, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<BalanceResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de los saldos de una cuenta asociada a un usuario de forma segura.B
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<BalanceResultInfo>> IBalanceInquiries<BalanceResultInfo>.GetBalancesAsync(
            string docType,
            string docNumber,
            string accountId)
        {
            return await Task.Run(() => this.InquiriesV11.GetBalances(
                docType,
                docNumber,
                accountId));
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula los saldos de una cuenta asociada a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{IBalanceInfo}" /> con la información del resultado por la solicitud de los saldos de una cuenta del usuario especificado.
        /// </returns>
        IList<BalanceResultInfo> IBalanceInquiries<BalanceResultInfo>.GetBalancesByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByAlias, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<BalanceResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula los saldos de una cuenta asociada a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<BalanceResultInfo>> IBalanceInquiries<BalanceResultInfo>.GetBalancesByAliasAsync(
            string channelId,
            string enrollmentAlias,
            string accountId)
        {
            return await Task.Run(() => this.InquiriesV11.GetBalancesByAlias(
                channelId,
                enrollmentAlias,
                accountId));
        }

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de los movimientos financieros de una cuenta asociada a un usuario de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{TData}" /> con la información del resultado por la solicitud de los saldos de una cuenta del usuario especificado.
        /// </returns>
        IList<MiniStatementResultInfo> IStatementInquiries<MiniStatementResultInfo>.GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByUserIdentity, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<MiniStatementResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado por la solicitud de los movimientos financieros de una cuenta asociada a un usuario de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<MiniStatementResultInfo>> IStatementInquiries<MiniStatementResultInfo>.GetStatementsAsync(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId)
        {
            return await Task.Run(() => this.InquiriesV11.GetStatements(
                docType,
                docNumber,
                accountId,
                accountTypeId));
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro procesado de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquiryResultInfo{IMiniStatementInfo}" /> con la información del resultado por la solicitud de los movimientos de una cuenta del usuario especificado.
        /// </returns>
        IList<MiniStatementResultInfo> IStatementInquiries<MiniStatementResultInfo>.GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByAlias, endpointParameters);
            ServiceLocator.Instance.HeadersManager.AddApiVersionHeader(request, "1.1");
            return this.Execute<List<MiniStatementResultInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información del resultado que encapsula los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro procesado de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        async Task<IList<MiniStatementResultInfo>> IStatementInquiries<MiniStatementResultInfo>.GetStatementsByAliasAsync(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId)
        {
            return await Task.Run(() => this.InquiriesV11.GetStatementsByAlias(
                channelId,
                enrollmentAlias,
                accountId,
                accountTypeId));
        }
    }
}