// -----------------------------------------------------------------------
// <copyright file="IAccountInquiries.cs" company="Evertec Colombia">
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
    /// <typeparam name="TResult">El tipo que representa al resultado de la consulta de cuentas.</typeparam>
    public interface IAccountInquiries<TResult> where TResult : class, new()
    {
        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Lista de instancias de <typeparamref name="TResult"/> con la información del resultado por la solicitud de cuentas del usuario especificado.
        /// </returns>
        IList<TResult> GetAccounts(string docType, string docNumber);

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<TResult>> GetAccountsAsync(string docType, string docNumber);

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de instancias de <typeparamref name="TResult"/> con la información de las cuentas del usuario a partir del alias especificado.
        /// </returns>
        IList<TResult> GetAccountsByAlias(string channelId, string enrollmentAlias);

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<TResult>> GetAccountsByAliasAsync(string channelId, string enrollmentAlias);
    }
}