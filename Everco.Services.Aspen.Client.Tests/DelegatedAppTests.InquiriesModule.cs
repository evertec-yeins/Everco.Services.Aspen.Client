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
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identity;
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

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            const string BackgroundColorPattern = @"^#(?:[0-9a-fA-F]{3}){1,2}$";
            foreach (AccountExtendedInfo account in accounts)
            {
                Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(account.Name, Is.Not.Empty);
                Assert.That(account.ShortName, Is.Not.Empty);
                Assert.That(account.BackgroundColor, Is.Not.Null.And.Match(BackgroundColorPattern));
                CollectionAssert.IsNotEmpty(account.Properties);
            }

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
            IList<BalanceExtendedInfo> balances = client.Inquiries.GetBalances(accountId);
            CollectionAssert.IsNotEmpty(balances);
            Assert.That(balances.Count, Is.EqualTo(5));
            const string AccountTypesPattern = "80|81|82|83|84";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (BalanceExtendedInfo balance in balances)
            {
                Assert.That(balance.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(balance.Number, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(balance.SourceAccountId, Is.Not.Empty);
                Assert.That(balance.TypeId, Is.Not.Null.And.Match(AccountTypesPattern));
                Assert.That(balance.TypeName, Is.Not.Empty);
            }
        }

        /// <summary>
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una respuesta exitosa sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesUnrecognizedAccountResponseIsEmpty()
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
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);
            IList<BalanceExtendedInfo> balances = client.Inquiries.GetBalances(accountId);
            CollectionAssert.IsNotEmpty(balances);

            // Los movimientos más recientes realizados por todas las cuentas...
            IList<MiniStatementInfo> statements = client.Inquiries.GetStatements(accountId);
            CollectionAssert.IsNotEmpty(statements);
            Assert.That(statements.Count, Is.EqualTo(5));
            const string AccountTypesPattern = "80|81|82|83|84";
            foreach (MiniStatementInfo statement in statements)
            {
                Assert.That(statement.AccountTypeId, Is.Not.Null.And.Match(AccountTypesPattern));
                Assert.That(statement.Amount, Is.AssignableTo(typeof(decimal)));
                Assert.That(statement.CardAcceptor, Is.Not.Null);
                Assert.That(statement.TranName, Is.Not.Null);
                Assert.That(statement.TranType, Is.Not.Null);
            }

            // Los movimientos realizados por una cuenta específica...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            IList<MiniStatementInfo> statementsByAccountType = client.Inquiries.GetStatements(accountId, accountTypeId);
            CollectionAssert.IsNotEmpty(statementsByAccountType);
            Assert.That(statements.Count, Is.EqualTo(5));
            foreach (MiniStatementInfo statement in statementsByAccountType)
            {
                Assert.That(statement.AccountTypeId, Is.EqualTo(accountTypeId));
                Assert.That(statement.Amount, Is.AssignableTo(typeof(decimal)));
                Assert.That(statement.CardAcceptor, Is.Not.Null);
                Assert.That(statement.TranName, Is.Not.Null);
                Assert.That(statement.TranType, Is.Not.Null);
            }
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta que no existe produce una una salida exitosa pero sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUnrecognizedAccountResponseIsEmpty()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string unrecognizedAccountId = new Random().Next(999, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(unrecognizedAccountId, "80");
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }

        /// <summary>
        /// Obtener los movimientos usando un tipo de cuenta que no existe produce una respuesta exitosa sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUnrecognizedAccountTypeResponseIsEmpty()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<AccountExtendedInfo> accounts = client.Inquiries.GetAccounts();
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);

            string unrecognizedAccountTypeId = new Random().Next(999, 9999).ToString();
            IList<MiniStatementInfo> statements = client.Inquiries.GetStatements(accountId, unrecognizedAccountTypeId);
            CollectionAssert.IsEmpty(statements);
        }

        /// <summary>
        /// Obtener las cuentas de los proveedores de datos conocidos.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsRecognizedDataProvidersRequestWorks()
        {
            // Se habilitan los proveedores actuales en la aplicación...
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP|Bancor");

            IDelegatedApp client = this.GetDelegatedClient();
            IList<AccountExtendedInfo> accounts = client.Inquiries.GetAccounts();
            CollectionAssert.IsNotEmpty(accounts);
            Assert.That(accounts.Count, Is.EqualTo(2));

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            const string BackgroundColorPattern = @"^#(?:[0-9a-fA-F]{3}){1,2}$";
            foreach (AccountExtendedInfo account in accounts)
            {
                Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(account.Name, Is.Not.Empty);
                Assert.That(account.ShortName, Is.Not.Empty);
                Assert.That(account.BackgroundColor, Is.Not.Null.And.Match(BackgroundColorPattern));
                CollectionAssert.IsNotEmpty(account.Properties);

                IList<AccountProperty> accountProperties = account.Properties;
                switch (account.Source)
                {
                    case Subsystem.Tup:
                        Assert.That(accountProperties.Count, Is.EqualTo(4));
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "LastTranName"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "LastTranDate"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "LastTranCardAcceptor"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "CardStatusName"), Is.Not.Null);
                        break;

                    case Subsystem.Bancor:
                        Assert.That(accountProperties.Count, Is.EqualTo(4));
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "LastTran"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "NextPayment"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "FullPayment"), Is.Not.Null);
                        Assert.That(accountProperties.SingleOrDefault(p => p.Key == "PartialPayment"), Is.Not.Null);
                        break;
                }
            }

            // Se reestablece la aplicación para usar el proveedor predeterminando para pruebas...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }
    }
}