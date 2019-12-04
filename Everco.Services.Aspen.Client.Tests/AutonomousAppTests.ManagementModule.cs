// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.ManagementModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-03 10:24 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Identity;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a entidades de administración para una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obtener las cuentas vinculadas para transferencia de saldos de un usuario.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetTransferAccountsWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            this.LinkTransferAccountsToRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            CollectionAssert.IsNotEmpty(transferAccounts);
            Assert.That(transferAccounts.Count, Is.EqualTo(5));
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
        /// Obtener las cuentas vinculadas cuando el usuario no tiene cuentas para transferencias vinculadas.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetTransferAccountsNoContentWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            CollectionAssert.IsEmpty(transferAccounts);
        }

        /// <summary>
        /// Obtener las cuentas vinculadas de un usuario no reconocido.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetTransferAccountsUnrecognizedUserWorks()
        {
            string fixedDocType = "CC";
            string unrecognizedDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(fixedDocType, unrecognizedDocNumber);
            CollectionAssert.IsEmpty(transferAccounts);
        }

        /// <summary>
        /// Obtener cuentas con tipos de documento no reconocidos para el usuario.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void GetTransferAccountsUnsupportedDocTypeWorks()
        {
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();

            string[] unsupportedDocTypes = { "01", "10", "xx", "Xx", "xX", "a1", "A1" };
            foreach (string unsupportedDocType in unsupportedDocTypes)
            {
                IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(unsupportedDocType, recognizedDocNumber);
                CollectionAssert.IsEmpty(transferAccounts);
            }
        }

        /// <summary>
        /// Vincular una cuenta reconocida a un usuario.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            string accountNumber = "6039590286132628";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                recognizedAlias,
                accountNumber);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
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
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            
            string recognizedCardHolderDocType = "CC";
            string unrecognizedCardHolderDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            LinkTransferAccountInfo unrecognizedTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                unrecognizedCardHolderDocNumber,
                "MyAccountAlias");

            IAutonomousApp client = this.GetAutonomousClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(
                recognizedDocType,
                recognizedDocNumber,
                unrecognizedTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15867"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
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
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            string unrecognizedAccountNumber = new Random().Next(1000000000, int.MaxValue).ToString("0000000000000000");
            LinkTransferAccountInfo unrecognizedTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                recognizedAlias,
                unrecognizedAccountNumber);

            IAutonomousApp client = this.GetAutonomousClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, unrecognizedTransferAccountInfo));
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
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string accountNumber = "6039590286132628";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                accountNumber: accountNumber);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
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
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string recognizedAlias = "MyTransferAccount";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(recognizedCardHolderDocType, recognizedCardHolderDocNumber, recognizedAlias);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
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
        /// No es posible vincular la cuenta del mismo tarjetahabiente.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountUserOwnerAndCardHolderAreEqualThrows()
        {
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string ownerDocType = recognizedUserIdentity.DocType;
            string ownerDocNumber = recognizedUserIdentity.DocNumber;
            string ownerAccountNumber = "6039597499604889";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(ownerDocType, ownerDocNumber, "MyAccount", ownerAccountNumber);
            IAutonomousApp client = this.GetAutonomousClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(ownerDocType, ownerDocNumber, linkTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15867"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("Tarjetahabiente origen y destino son el mismo", exception.Message);
        }

        /// <summary>
        /// Se puede vincular un máximo de cuentas a un usuario.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountsMaxAllowedExceededThrows()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            this.DeleteTransferAccountsFromRecognizedUser();
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "TransferAccounts:MaxRegisterAllowed", "5");
            this.LinkTransferAccountsToRecognizedUser();
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            CollectionAssert.IsNotEmpty(transferAccounts);
            Assert.That(transferAccounts.Count, Is.EqualTo(5));

            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "52099375";
            TransferAccountInfo unlinkedTransferAccountInfo = transferAccounts.FirstOrDefault(
                info => info.CardHolderDocType == recognizedCardHolderDocType
                        & info.CardHolderDocNumber == recognizedCardHolderDocNumber);
            Assert.IsNull(unlinkedTransferAccountInfo);

            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(recognizedCardHolderDocType, recognizedCardHolderDocNumber);
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15854"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch(Regex.Escape("Excede el número máximo de cuentas registradas (5)"), exception.Message);
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "TransferAccounts:MaxRegisterAllowed", "10");
        }

        /// <summary>
        /// Vincular una cuenta reconocida sin alias y sin número de cuenta del titular.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountMissingAliasAndAccountNumberWorks()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;
            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(recognizedCardHolderDocType, recognizedCardHolderDocNumber);
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
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
        /// Vincular la información de una cuenta usando un alias usado por otra cuenta.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountAlreadyExistAliasThrows()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            string recognizedCardHolderDocType1 = "CC";
            string recognizedCardHolderDocNumber1 = "79483129";
            string alreadyExistAlias = "MyTransferAccount";
            LinkTransferAccountInfo linkTransferAccountInfo1 = new LinkTransferAccountInfo(
                recognizedCardHolderDocType1,
                recognizedCardHolderDocNumber1,
                alreadyExistAlias);
            IAutonomousApp client = this.GetAutonomousClient();
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo1));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            CollectionAssert.IsNotEmpty(transferAccounts);
            TransferAccountInfo transferAccountInfo = transferAccounts.FirstOrDefault(info => info.Alias == alreadyExistAlias);
            Assert.NotNull(transferAccountInfo);

            string recognizedCardHolderDocType2 = "CC";
            string recognizedCardHolderDocNumber2 = "35512889";
            LinkTransferAccountInfo linkTransferAccountInfo2 = new LinkTransferAccountInfo(
                recognizedCardHolderDocType2,
                recognizedCardHolderDocNumber2,
                alreadyExistAlias);
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(
                recognizedDocType,
                recognizedDocNumber,
                linkTransferAccountInfo2));
            Assert.That(exception.EventId, Is.EqualTo("15853"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch($"Ya existe una cuenta registrada con el nombre '{alreadyExistAlias}'", exception.Message);
        }

        /// <summary>
        /// Vincular la información de una tarjetahabiente que ya existe.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountAlreadyExistCardHolderThrows()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string randomAlias1 = $"MyRandomAlias-{new Random().Next(9999, 99999)}";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                randomAlias1);
            IAutonomousApp client = this.GetAutonomousClient();
            Assert.DoesNotThrow(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            IList<TransferAccountInfo> transferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            CollectionAssert.IsNotEmpty(transferAccounts);
            TransferAccountInfo transferAccountInfo = transferAccounts.FirstOrDefault(info => info.CardHolderDocNumber == recognizedCardHolderDocNumber);
            Assert.NotNull(transferAccountInfo);

            string randomAlias2 = $"MyRandomAlias-{new Random().Next(9999, 99999)}";
            linkTransferAccountInfo.Alias = randomAlias2;
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15863"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch("Cuenta ya está registrada con otro alias", exception.Message);
        }

        /// <summary>
        /// Vincular una cuenta a un usuario con un tipo de documento desconocido.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountEmptyOwnerDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string emptyDocType = "     ";
            string recognizedDocNumber = RecognizedUserIdentity.Master.DocNumber;
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                recognizedCardHolderDocType,
                recognizedCardHolderDocNumber,
                "MyAlias");
            AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(
                emptyDocType,
                recognizedDocNumber,
                linkTransferAccountInfo));
            Assert.That(exception.EventId, Is.EqualTo("15867"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("'UserDocType' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Vincular una cuenta a un usuario con un tipo de documento desconocido.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountUnrecognizedOwnerDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedDocNumber = RecognizedUserIdentity.Master.DocNumber;
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";
            string[] unrecognizedDocTypes = { "01", "10", "xx", "Xx", "xX", "a1", "A1" };

            foreach (string unrecognizedDocType in unrecognizedDocTypes)
            {
                LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                    recognizedCardHolderDocType,
                    recognizedCardHolderDocNumber,
                    "MyAlias");
                AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(
                    unrecognizedDocType,
                    recognizedDocNumber,
                    linkTransferAccountInfo));
                Assert.That(exception.EventId, Is.EqualTo("15867"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
                StringAssert.IsMatch($"Tipo de documento en la URL no es reconocido. '{unrecognizedDocType}' no es reconocido como tipo de documento", exception.Message);
            }
        }

        /// <summary>
        /// Vincular la información de una tarjetahabiente usando un alias inválido.
        /// </summary>
        [Test]
        [Category("Modules.Management.TransferAccounts")]
        public void LinkTransferAccountInvalidFormatAliasThrows()
        {
            this.DeleteTransferAccountsFromRecognizedUser();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            IAutonomousApp client = this.GetAutonomousClient();
            string recognizedCardHolderDocType = "CC";
            string recognizedCardHolderDocNumber = "79483129";

            string[] invalidFormatAlias =
                {
                    "Lórem ípsum dolor sit ámet", // NO accents.
                    "<gXjyhrYqannHUA$LLV&7guTHm.F&1X5JB$Uobx3@!rPn9&x;4BzE>", // NO special characters.
                    $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}" // Exceeded maximum length (150)
                };
            foreach (string invalidAlias in invalidFormatAlias)
            {
                LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(
                    recognizedCardHolderDocType,
                    recognizedCardHolderDocNumber,
                    invalidAlias);
                AspenException exception = Assert.Throws<AspenException>(() => client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo));
                Assert.That(exception.EventId, Is.EqualTo("15867"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
                StringAssert.IsMatch(Regex.Escape("'Alias' incluye caracteres inválidos o excede la longitud máxima (150). Por favor utilice letras (sin acentos), números y guiones"), exception.Message);
            }
        }

        /// <summary>
        /// Restablece y registra cuentas para transferencia al usuario de pruebas reconocido.
        /// </summary>
        private void LinkTransferAccountsToRecognizedUser()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

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
                LinkTransferAccountInfo linkTransferAccountInfo = new LinkTransferAccountInfo(cardHolderDocType, cardHolderDocNumber);
                client.Management.LinkTransferAccount(recognizedDocType, recognizedDocNumber, linkTransferAccountInfo);
            }
        }

        /// <summary>
        /// Restablece y registra cuentas para transferencia al usuario de pruebas reconocido.
        /// </summary>
        private void DeleteTransferAccountsFromRecognizedUser()
        {
            IUserIdentity recognizedUserIdentity = RecognizedUserIdentity.Master;
            string recognizedDocType = recognizedUserIdentity.DocType;
            string recognizedDocNumber = recognizedUserIdentity.DocNumber;

            IAutonomousApp client = this.GetAutonomousClient();
            IList<TransferAccountInfo> currentTransferAccounts = client.Management.GetTransferAccounts(recognizedDocType, recognizedDocNumber);
            foreach (TransferAccountInfo transferAccountInfo in currentTransferAccounts)
            {
                client.Management.UnlinkTransferAccount(recognizedDocType, recognizedDocNumber, transferAccountInfo.Alias);
            }
        }
    }
}