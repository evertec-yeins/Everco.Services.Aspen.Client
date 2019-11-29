// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Management.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using Entities;
    using Everco.Services.Aspen.Client.Internals;
    using Modules.Autonomous;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : IManagementModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones de administración varias.
        /// </summary>
        public IManagementModule Management => this;

        /// <summary>
        /// Obtiene la información de las cuentas vinculadas a un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se obtienen las cuentas.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se obtienen las cuentas.</param>
        /// <returns>Listado de cuentas inscritas.</returns>
        /// <returns>
        /// Lista de instancias de <see cref="ITransferAccountInfo" /> con la información de las cuentas vinculadas al usuario especificado.
        /// </returns>
        public IList<TransferAccountInfo> GetTransferAccounts(string docType, string docNumber)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddOwnerDocType(docType)
                .AddOwnerDocNumber(docNumber);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TransferAccountsByUserIdentity, endpointParameters);
            return this.Execute<List<TransferAccountInfo>>(request);
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta que se está vinculando.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta que se está vinculando.</param>
        /// <param name="accountNumber">El número de cuenta que se está vinculando o <see langword="null" /> para usar el primer número de cuenta asociado con <paramref name="cardHolderDocType" /> y <paramref name="cardHolderDocNumber" />.</param>
        /// <param name="alias">El nombre o alias con el que se desea identificar a la cuenta que se está vinculando o <see langword="null" /> para usar la combinación de <paramref name="cardHolderDocType" /> y <paramref name="cardHolderDocNumber" />.</param>
        public void LinkTransferAccount(
            string docType,
            string docNumber,
            string cardHolderDocType,
            string cardHolderDocNumber,
            string accountNumber = null,
            string alias = null)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Vincula la información de una cuenta a las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculará la cuenta.</param>
        /// <param name="accountInfo">La información de la cuenta a vincular.</param>
        public void LinkTransferAccount(string docType, string docNumber, ILinkTransferAccountInfo accountInfo)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="alias">El nombre o alias con el que se vinculó la cuenta.</param>
        public void UnlinkTransferAccount(string docType, string docNumber, string alias)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Desvincula la información de una cuenta de las cuentas inscritas de un usuario para transferencia de saldos.
        /// </summary>
        /// <param name="docType">El tipo de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="docNumber">El número de documento del cliente al cual se vinculó la cuenta.</param>
        /// <param name="cardHolderDocType">El tipo de documento del titular de la cuenta.</param>
        /// <param name="cardHolderDocNumber">El número de documento del titular de la cuenta.</param>
        public void UnlinkTransferAccount(
            string docType,
            string docNumber,
            string cardHolderDocType,
            string cardHolderDocNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}