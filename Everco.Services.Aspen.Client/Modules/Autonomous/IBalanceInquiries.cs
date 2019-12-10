// -----------------------------------------------------------------------
// <copyright file="IBalanceInquiries.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-10 07:15 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;

    /// <summary>
    /// Define las consultas de los productos financieros soportadas para una aplicación con alcance de autónoma.
    /// </summary>
    /// <typeparam name="TResult">El tipo que representa al resultado de la consulta de saldos de una cuenta.</typeparam>
    public interface IBalanceInquiries<TResult> where TResult : class, new()
    {
        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <typeparamref name="TResult"/> con la información de saldos de las cuentas del usuario especificado.
        /// </returns> 
        IList<TResult> GetBalances(string docType, string docNumber, string accountId);

        /// <summary>
        /// Obtiene la información de saldos de una cuenta asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de instancias de <typeparamref name="TResult"/> con la información de saldos de las cuentas del usuario a partir del alias especificado.
        /// </returns>
        IList<TResult> GetBalancesByAlias(string channelId, string enrollmentAlias, string accountId);
    }
}