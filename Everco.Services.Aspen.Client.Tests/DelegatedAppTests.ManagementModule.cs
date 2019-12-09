// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.ManagementModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
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
        public void GetTransferAccountsNoContentWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsEmpty(transferAccounts);
        }

        /// <summary>
        /// Vincular una cuenta reconocida a un usuario.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            string recognizedPinNumber = "141414";
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            string accountNumber = "6039590286132628";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                recognizedAlias,
                accountNumber,
                recognizedPinNumber);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsNotEmpty(transferAccounts);
            TransferAccountInfo transferAccountInfo = transferAccounts.FirstOrDefault(info => info.Alias == recognizedAlias);
            Assert.NotNull(transferAccountInfo);
            Assert.That(transferAccountInfo.CardHolderDocType, Is.EqualTo(recognizedCardHolderDocType));
            Assert.That(transferAccountInfo.CardHolderDocNumber, Is.EqualTo(recognizedCardHolderDocNumber));
            Assert.IsNotEmpty(transferAccountInfo.CardHolderName);
            const string MaskedPanPattern = @".*\d{4}";
            Assert.That(transferAccountInfo.MaskedPan, Is.Not.Null.And.Match(MaskedPanPattern));
        }

        /// <summary>
        /// Vincular la información de un titular de cuenta no reconocido.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountUnrecognizedCardHolderThrows()
        {
            string recognizedPinNumber = "141414";
            string recognizedCardHolderDocType = "CC";
            string unrecognizedCardHolderDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string randomAlias = $"MyAlias-{new Random().Next(9999, 99999)}";
            string randomAccountNumber = "6039590286132628";
            LinkTransferAccountInfo unrecognizedTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                unrecognizedCardHolderDocNumber,
                randomAlias,
                randomAccountNumber,
                recognizedPinNumber);
            IDelegatedApp client = this.GetDelegatedClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(unrecognizedTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15856"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch("No se encuentra información de la cuenta con los datos suministrados", exception.Message);
        }

        /// <summary>
        /// Vincular la cuenta de un tarjetahabiente desconocida.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountUnrecognizedAccountNumberThrows()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            string recognizedPinNumber = "141414";
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            string unrecognizedAccountNumber = new Random().Next(1000000000, int.MaxValue).ToString("0000000000000000");
            LinkTransferAccountInfo unrecognizedTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                recognizedAlias,
                unrecognizedAccountNumber,
                recognizedPinNumber);

            IDelegatedApp client = this.GetDelegatedClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(unrecognizedTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15857"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch("Cuenta no pertenece a tarjetahabiente", exception.Message);
        }

        /// <summary>
        /// Vincular una cuenta reconocida sin alias.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountMissingAliasWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            string recognizedPinNumber = "141414";
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string accountNumber = "6039590286132628";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                null,
                accountNumber,
                recognizedPinNumber);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsNotEmpty(transferAccounts);
            TransferAccountInfo transferAccountInfo = transferAccounts.FirstOrDefault(info => info.CardHolderDocNumber == recognizedCardHolderDocNumber);
            Assert.NotNull(transferAccountInfo);
            string defaultAlias = $"{recognizedCardHolderDocType}-{recognizedCardHolderDocNumber}";
            Assert.That(transferAccountInfo.Alias, Is.Not.Null.And.EqualTo(defaultAlias));
            Assert.That(transferAccountInfo.CardHolderDocType, Is.EqualTo(recognizedCardHolderDocType));
            Assert.That(transferAccountInfo.CardHolderDocNumber, Is.EqualTo(recognizedCardHolderDocNumber));
            Assert.IsNotEmpty(transferAccountInfo.CardHolderName);
            const string MaskedPanPattern = @".*\d{4}";
            Assert.That(transferAccountInfo.MaskedPan, Is.Not.Null.And.Match(MaskedPanPattern));
        }

        /// <summary>
        /// Vincular una cuenta reconocida sin número de cuenta del titular.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountMissingAccountNumberWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IDelegatedApp client = this.GetDelegatedClient();
            string recognizedPinNumber = "141414";
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                recognizedPinNumber,
                alias: recognizedAlias));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts();
            CollectionAssert.IsNotEmpty(transferAccounts);
            TransferAccountInfo transferAccountInfo = transferAccounts.FirstOrDefault(info => info.CardHolderDocNumber == recognizedCardHolderDocNumber);
            Assert.NotNull(transferAccountInfo);
            Assert.That(transferAccountInfo.Alias, Is.Not.Null.And.EqualTo(recognizedAlias));
            Assert.That(transferAccountInfo.CardHolderDocType, Is.EqualTo(recognizedCardHolderDocType));
            Assert.That(transferAccountInfo.CardHolderDocNumber, Is.EqualTo(recognizedCardHolderDocNumber));
            Assert.IsNotEmpty(transferAccountInfo.CardHolderName);
            const string MaskedPanPattern = @".*\d{4}";
            Assert.That(transferAccountInfo.MaskedPan, Is.Not.Null.And.Match(MaskedPanPattern));
        }

        /// <summary>
        /// Restablece y registra cuentas para transferencia al usuario de pruebas reconocido.
        /// </summary>
        private void LinkTransferAccountsToRecognizedUser()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            List<(string, string, string)> recognizedCardHolders = new List<(string, string, string)>
                                                               {
                                                                   ("CC", "79483129", "6039590286132628"),
                                                                   ("CC", "52150900", "6039594610201554"),
                                                                   ("CC", "3262308", "6039590635149836"),
                                                                   ("CC", "52582664", "6039594246705697"),
                                                                   ("CC", "35512889", "6039599272117600")
                                                               };

            const string RecognizedPinNumber = "141414";
            foreach ((string cardHolderDocType, string cardHolderDocNumber, string accountNumber) in recognizedCardHolders)
            {
                string randomAlias = new Random().Next(9999, 99999).ToString();
                LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                    cardHolderDocType,
                    cardHolderDocNumber,
                    randomAlias,
                    accountNumber,
                    RecognizedPinNumber);
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