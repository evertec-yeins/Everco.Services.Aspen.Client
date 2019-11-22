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
    using System.Net;
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            IList<AccountExtendedInfo> GetAccounts() => client.Inquiries.GetAccounts();
            Assert.DoesNotThrow(() => GetAccounts());
            CollectionAssert.IsNotEmpty(GetAccounts());
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesRequestWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
            
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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            string randomAccountId = new Random().Next(99, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(randomAccountId);
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }
    }
}