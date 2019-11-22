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
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
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
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            IUserIdentity userIdentity = UserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;

            IList<AccountInfo> GetAccounts() => client.Inquiries.GetAccounts(docType, docNumber);
            Assert.DoesNotThrow(() => GetAccounts());
            CollectionAssert.IsNotEmpty(GetAccounts());
        }

        /// <summary>
        /// Obtener las cuentas de un usuario que no existe produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsFromUnrecognizedUserRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();

            IList<AccountInfo> GetAccounts() => client.Inquiries.GetAccounts(fixedDocType, randomDocNumber);
            Assert.DoesNotThrow(() => GetAccounts());
            CollectionAssert.IsEmpty(GetAccounts());
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            IUserIdentity userIdentity = UserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo tupAccountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(tupAccountInfo);
            string accountId = tupAccountInfo.SourceAccountId;
            Assert.DoesNotThrow(() => client.Inquiries.GetBalances(docType, docNumber, accountId));
        }

        /// <summary>
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una salida válida.
        /// </summary>|
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesFromUnrecognizedUserRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string randomAccountId = new Random().Next(99, 9999).ToString();

            IList<BalanceInfo> GetBalance() => client.Inquiries.GetBalances(fixedDocType, randomDocNumber, randomAccountId);
            Assert.DoesNotThrow(() => GetBalance());
            CollectionAssert.IsEmpty(GetBalance());
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            IUserIdentity userIdentity = UserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;

            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo tupAccountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(tupAccountInfo);
            string accountId = tupAccountInfo.SourceAccountId;
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);
            
            // Los últimos movimientos por toda las cuentas...
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(docType, docNumber, accountId));

            // Los movimientos de una cuenta específica...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            Assert.DoesNotThrow(() => client.Inquiries.GetStatements(docType, docNumber, accountId, accountTypeId));
        }

        /// <summary>
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una salida válida.
        /// </summary>|
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsFromUnrecognizedUserRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string randomAccountId = new Random().Next(99, 9999).ToString();

            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(fixedDocType, randomDocNumber, randomAccountId);
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }
    }
}