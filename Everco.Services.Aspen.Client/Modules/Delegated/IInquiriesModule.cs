// -----------------------------------------------------------------------
// <copyright file="IInquiriesModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 17:25 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Delegated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    /// <summary>
    /// Define las consultas soportadas de los productos financieros del usuario autenticado.
    /// </summary>
    public interface IInquiriesModule
    {
        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas al usuario actual.
        /// </summary>
        /// <returns>
        /// Lista de tipo <see cref="IAccountExtendedInfo"/> con la información de las cuentas del usuario actual.
        /// </returns>
        IList<AccountExtendedInfo> GetAccounts();

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas al usuario actual.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<AccountExtendedInfo>> GetAccountsAsync();

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de tipo <see cref="IBalanceExtendedInfo" /> con la información de saldos de las cuentas del usuario actual.
        /// </returns>
        IList<BalanceExtendedInfo> GetBalances(string accountId);

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<BalanceExtendedInfo>> GetBalancesAsync(string accountId);

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada del usuario actual.
        /// </returns>
        IList<MiniStatementInfo> GetStatements(string accountId, string accountTypeId = null);

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada al usuario actual.
        /// </summary>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada del usuario actual.
        /// </returns>
        Task<IList<MiniStatementInfo>> GetStatementsAsync(string accountId, string accountTypeId = null);
    }
}