// -----------------------------------------------------------------------
// <copyright file="IManagementModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-29 10:55 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las operaciones soportadas para acceder a entidades de administración para una aplicación con alcance de autónoma.
    /// </summary>
    public interface IManagementModule
    {
        /// <summary>
        /// Obtiene la información de las cuentas vinculadas a un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se obtienen las cuentas.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se obtienen las cuentas.</param>
        /// <returns>Listado de cuentas inscritas.</returns>
        /// <returns>
        /// Lista de instancias de <see cref="ITransferAccountInfo" /> con la información de las cuentas vinculadas al usuario especificado.
        /// </returns>
        IList<TransferAccountInfo> GetTransferAccounts(string docType, string docNumber);

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta que se está vinculando.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta que se está vinculando.</param>
        /// <param name="accountNumber">El número de cuenta que se está vinculando o <see langword="null" /> para usar el primer número de cuenta asociado con <paramref name="cardHolderDocType"/> y <paramref name="cardHolderDocNumber"/>.</param>
        /// <param name="alias">El nombre o alias con el que se desea identificar a la cuenta que se está vinculando o <see langword="null" /> para usar la combinación de <paramref name="cardHolderDocType"/> y <paramref name="cardHolderDocNumber"/>.</param>
        void LinkTransferAccount(
            string docType,
            string docNumber,
            string cardHolderDocType,
            string cardHolderDocNumber,
            string accountNumber = null,
            string alias = null);

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        void LinkTransferAccount(string docType, string docNumber, ILinkTransferAccountInfo accountInfo);

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        void UnlinkTransferAccount(string docType, string docNumber, string alias);

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta.</param>
        void UnlinkTransferAccount(
            string docType,
            string docNumber,
            string cardHolderDocType,
            string cardHolderDocNumber);
    }
}