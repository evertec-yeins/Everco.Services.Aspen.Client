// -----------------------------------------------------------------------
// <copyright file="IStatementInquiries.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-10 07:15 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Define las consultas de los productos financieros soportadas para una aplicación con alcance de autónoma.
    /// </summary>
    /// <typeparam name="TResult">El tipo que representa al resultado de la consulta de movimientos de una cuenta.</typeparam>
    public interface IStatementInquiries<TResult> where TResult : class, new()
    {
        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de instancias de <typeparamref name="TResult"/> con la información de los movimientos financieros de la cuenta asociada al usuario especificado.
        /// </returns>
        IList<TResult> GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null);

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
        Task<IList<TResult>> GetStatementsAsync(
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
        /// Lista de instancias de <typeparamref name="TResult"/> con la información de los movimientos financieros de la cuenta asociada al usuario a partir del alias especificado.
        /// </returns>
        IList<TResult> GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
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
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<TResult>> GetStatementsByAliasAsync(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null);
    }
}