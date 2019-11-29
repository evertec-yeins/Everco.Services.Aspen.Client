// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.InquiriesModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 15:06 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
    using Identity;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias de las consultas de los productos financieros del usuario.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obtener las cuentas de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            Assert.That(accounts.Count, Is.EqualTo(1));
            IList<AccountProperty> properties = accounts.First().Properties.ToList();
            Assert.That(properties.Count, Is.EqualTo(4));
            Assert.That(properties.SingleOrDefault(p => p.Key == "LastTranName"), Is.Not.Null);
            Assert.That(properties.SingleOrDefault(p => p.Key == "LastTranDate"), Is.Not.Null);
            Assert.That(properties.SingleOrDefault(p => p.Key == "LastTranCardAcceptor"), Is.Not.Null);
            Assert.That(properties.SingleOrDefault(p => p.Key == "CardStatusName"), Is.Not.Null);
        }

        /// <summary>
        /// Obtener las cuentas de un usuario que no existe produce una salida exitosa sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsUsingUnrecognizedUserThenResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(fixedDocType, randomDocNumber);
            CollectionAssert.IsEmpty(accounts);
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);
            Assert.That(balances.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una respuesta exitosa sin resultados.
        /// </summary>|
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesUsingUnrecognizedUserThenResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string randomAccountId = new Random().Next(99, 9999).ToString();
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(fixedDocType, randomDocNumber, randomAccountId);
            CollectionAssert.IsEmpty(balances);
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo tupAccountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(tupAccountInfo);
            string accountId = tupAccountInfo.SourceAccountId;
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);
            
            // Los movimientos más recientes realizados entre todas las cuentas...
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(docType, docNumber, accountId));

            // Los movimientos realizados por una cuenta específica...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(docType, docNumber, accountId, accountTypeId));
        }

        /// <summary>
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una salida exitosa pero sin resultados.
        /// </summary>|
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUsingUnrecognizedUserThenResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string randomAccountId = new Random().Next(99, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(fixedDocType, randomDocNumber, randomAccountId);
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUnrecognizedAccountRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string randomAccountId = new Random().Next(99, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(randomAccountId);
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }
    }
}