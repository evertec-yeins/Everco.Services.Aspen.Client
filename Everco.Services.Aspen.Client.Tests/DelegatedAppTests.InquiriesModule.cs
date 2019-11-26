// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.InquiriesModule.cs" company="Evertec Colombia">
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
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias de las consultas de los productos financieros del usuario autenticado.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtener las cuentas de usuario actual produce una salida válida.|
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<AccountExtendedInfo> accounts = client.Inquiries.GetAccounts();
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
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<AccountExtendedInfo> accounts = client.Inquiries.GetAccounts();
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo tupAccountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(tupAccountInfo);
            string accountId = tupAccountInfo.SourceAccountId;
            Assert.DoesNotThrow(() => client.Inquiries.GetBalances(accountId));
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesUnrecognizedAccountRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string randomAccountId = new Random().Next(99, 9999).ToString();
            IList<BalanceExtendedInfo> GetBalances() => client.Inquiries.GetBalances(randomAccountId);
            Assert.DoesNotThrow(() => GetBalances());
            CollectionAssert.IsEmpty(GetBalances());
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<AccountExtendedInfo> accounts = client.Inquiries.GetAccounts();
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo tupAccountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(tupAccountInfo);
            string accountId = tupAccountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);
            IList<BalanceExtendedInfo> balances = client.Inquiries.GetBalances(accountId);
            CollectionAssert.IsNotEmpty(balances);

            // Los últimos movimientos por toda las cuentas...
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(accountId));

            // Los movimientos de una cuenta específica...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(accountId, accountTypeId));
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