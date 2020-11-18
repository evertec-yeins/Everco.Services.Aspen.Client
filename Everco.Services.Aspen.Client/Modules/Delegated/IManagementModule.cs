// -----------------------------------------------------------------------
// <copyright file="IManagementModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-29 10:55 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Delegated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    /// <summary>
    /// Define las operaciones soportadas para acceder a entidades de administración para una aplicación con alcance de delegada.
    /// </summary>
    public interface IManagementModule
    {
        /// <summary>
        /// Obtiene la información de las cuentas vinculadas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <returns>
        /// Lista de instancias de <see cref="ITransferAccountInfo" /> con la información de las cuentas vinculadas al usuario actual.
        /// </returns>
        IList<TransferAccountInfo> GetTransferAccounts();

        /// <summary>
        /// Obtiene la información de las cuentas vinculadas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<TransferAccountInfo>> GetTransferAccountsAsync();

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta que se desea registrar.</param>
        /// <param name="pinNumber">El pin transaccional del usuario autenticado.</param>
        /// <param name="accountNumber">El número de cuenta que se desea registrar o <c>null</c> para que el sistema busque el número de cuenta asociado con el tarjetahabiente.</param>
        /// <param name="alias">El nombre con el que se desea identificar la cuenta a registrar.</param>
        void LinkTransferAccount(
            string cardHolderDocType,
            string cardHolderDocNumber,
            string pinNumber,
            string accountNumber = null,
            string alias = null);

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
        Task LinkTransferAccountAsync(
            string cardHolderDocType,
            string cardHolderDocNumber,
            string pinNumber,
            string accountNumber = null,
            string alias = null);

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        void LinkTransferAccount(ILinkTransferAccountInfo accountInfo);

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas para transferencia de saldos al usuario actual.
        /// </summary>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task LinkTransferAccountAsync(ILinkTransferAccountInfo accountInfo);

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas para transferencia de saldos del usuario actual.
        /// </summary>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        void UnlinkTransferAccount(string alias);

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas para transferencia de saldos del usuario actual.
        /// </summary>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task UnlinkTransferAccountAsync(string alias);
    }
}