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
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IAccountInfo}" /> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<AccountInfo> GetAccounts(string docType, string docNumber);

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IAccountInfo"/> con la información de las cuentas del usuario especificado.
        /// </returns>
        IList<AccountInfo> GetAccountsByAlias(string channelId, string enrollmentAlias);

        /// <summary>
        /// Obtiene la información del resultado que encapsula el resumen de las cuentas asociadas a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IAccountInfo}" /> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<AccountSafeInfo> GetAccountsByAliasSafely(string channelId, string enrollmentAlias);

        /// <summary>
        /// Obtiene la información del resultado que encapsula el resumen de las cuentas asociadas a un usuario procesada de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IAccountInfo}"/> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<AccountSafeInfo> GetAccountsSafely(string docType, string docNumber);

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario especificado.
        /// </returns> 
        IList<BalanceInfo> GetBalances(string docType, string docNumber, string accountId);

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IBalanceInfo"/> con la información de saldos de las cuentas del usuario especificado.
        /// </returns>
        IList<BalanceInfo> GetBalancesByAlias(string channelId, string enrollmentAlias, string accountId);

        /// <summary>
        /// Obtiene la información del resultado que encapsula los saldos de una cuenta asociada a un usuario a partir de su alias de registro procesada de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IBalanceInfo}" /> con la información del resultado por la solicitud de los saldos de una cuenta del usuario especificado.
        /// </returns>
        IList<BalanceSafeInfo> GetBalancesByAliasSafely(string channelId, string enrollmentAlias, string accountId);

        /// <summary>
        /// Obtiene la información del resultado que encapsula los saldos de una cuenta asociada a un usuario procesada de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IBalanceInfo}" /> con la información del resultado por la solicitud de los saldos de una cuenta del usuario especificado.
        /// </returns>
        IList<BalanceSafeInfo> GetBalancesSafely(string docType, string docNumber, string accountId);

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada para el usuario.
        /// </returns>
        IList<MiniStatementInfo> GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null);

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada para el alias.
        /// </returns>
        IList<MiniStatementInfo> GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null);

        /// <summary>
        /// Obtiene la información del resultado que encapsula los movimientos financieros de una cuenta asociada a un usuario a partir de su alias de registro procesado de forma segura.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IMiniStatementInfo}" /> con la información del resultado por la solicitud de los movimientos de una cuenta del usuario especificado.
        /// </returns>
        IList<MiniStatementSafeInfo> GetStatementsByAliasSafely(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null);

        /// <summary>
        /// Obtiene la información del resultado que encapsula los movimientos financieros de una cuenta asociada a un usuario procesado de forma segura.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <see cref="IInquirySafeInfo{IMiniStatementInfo}" /> con la información del resultado por la solicitud de los movimientos de una cuenta del usuario especificado.
        /// </returns>
        IList<MiniStatementSafeInfo> GetStatementsSafely(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null);
    }
}