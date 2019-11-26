// -----------------------------------------------------------------------
// <copyright file="IInquiriesModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 17:25 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las consultas soportadas de los productos financieros para una aplicación con alcance de autónoma.
    /// </summary>
    public interface IInquiriesModule
    {
        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>Lista de tipo <see cref="IAccountInfo"/> con la información de las cuentas del usuario especificado.</returns>
        IList<AccountInfo> GetAccounts(string docType, string docNumber);

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>Lista de tipo <see cref="IAccountInfo"/> con la información de las cuentas del usuario especificado.</returns>
        IList<AccountInfo> GetAccountsByAlias(string channelId, string enrollmentAlias);

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>Lista de tipo <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario especificado.</returns>
        IList<BalanceInfo> GetBalances(string docType, string docNumber, string accountId);

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>Lista de tipo <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario especificado.</returns>
        IList<BalanceInfo> GetBalancesByAlias(string channelId, string enrollmentAlias, string accountId);

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada para el usuario.
        /// </returns>
        IList<MiniStatementInfo> GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null);

        /// <summary>
        /// Obtiene la información de movimientos de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>Listado con la información de movimientos financieros de la cuenta especificada para el usuario actual.</returns>
        IList<MiniStatementInfo> GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null);
    }
}