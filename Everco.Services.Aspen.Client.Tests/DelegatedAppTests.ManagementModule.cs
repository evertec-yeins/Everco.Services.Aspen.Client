// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.ManagementModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a entidades de administración para una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtener las cuentas vinculadas para transferencia de saldos del usuario actual.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetTransferAccountsWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            this.LinkTransferAccountsToRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsNotEmpty(transferAccounts);
            const string MaskedPanPattern = @".*\d{4}";
            foreach (TransferAccountInfo transferAccountInfo in transferAccounts)
            {
                Assert.NotNull(transferAccountInfo);
                Assert.IsNotEmpty(transferAccountInfo.Alias);
                Assert.IsNotEmpty(transferAccountInfo.CardHolderDocType);
                Assert.IsNotEmpty(transferAccountInfo.CardHolderDocNumber);
                Assert.IsNotEmpty(transferAccountInfo.CardHolderName);
                Assert.That(transferAccountInfo.MaskedPan, Is.Not.Null.And.Match(MaskedPanPattern));
            }
        }

        /// <summary>
        /// Obtener las cuentas vinculadas cuando el usuario aún no tiene cuentas para transferencias vinculadas.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetUnlinkedTransferAccountsWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsEmpty(transferAccounts);
        }

        /// <summary>
        /// Restablece y registra cuentas para transferencia al usuario de pruebas reconocido.
        /// </summary>
        private void LinkTransferAccountsToRecognizedUser()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            List<(string, string)> recognizedCardHolders = new List<(string, string)>
                                                               {
                                                                   ("CC", "79483129"),
                                                                   ("CC", "52150900"),
                                                                   ("CC", "3262308"),
                                                                   ("CC", "52582664"),
                                                                   ("CC", "35512889")
                                                               };

            foreach ((string cardHolderDocType, string cardHolderDocNumber) in recognizedCardHolders)
            {
                LinkTransferAccountPinSignedInfo linkTransferAccountInfo = new LinkTransferAccountPinSignedInfo(cardHolderDocType, cardHolderDocNumber, "141414");
                client.Management.LinkTransferAccount(linkTransferAccountInfo);
            }
        }

        /// <summary>
        /// Restablece y registra cuentas para transferencia al usuario de pruebas reconocido.
        /// </summary>
        private void DeleteTransferAccountsFromRecognizedUser()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<TransferAccountInfo> currentTransferAccounts = client.Management.GetTransferAccounts();
            foreach (TransferAccountInfo transferAccountInfo in currentTransferAccounts)
            {
                client.Management.UnlinkTransferAccount(transferAccountInfo.Alias);
            }
        }
    }
}