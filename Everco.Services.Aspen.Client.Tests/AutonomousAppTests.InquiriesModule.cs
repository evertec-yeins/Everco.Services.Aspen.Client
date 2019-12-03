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
    using System.Net;
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

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (AccountInfo account in accounts)
            {
                Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(account.Name, Is.Not.Empty);
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
        /// Obtener las cuentas de un usuario que no existe produce una salida exitosa sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsUnrecognizedUserResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string unrecognizedDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(fixedDocType, unrecognizedDocNumber);
            CollectionAssert.IsEmpty(accounts);
        }

        /// <summary>
        /// Obtener las cuentas por el alias de registro de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsByAliasRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            IList<AccountInfo> accounts = client.Inquiries.GetAccountsByAlias(channelId, enrollmentAlias);
            CollectionAssert.IsNotEmpty(accounts);
            Assert.That(accounts.Count, Is.EqualTo(1));

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (AccountInfo account in accounts)
            {
                Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(account.Name, Is.Not.Empty);
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
        /// Se produce una excepción cuando se intenta obtener las cuentas usando un alias de registro que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsByAliasUnrecognizedEnrollmentAliasThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string unrecognizedEnrollmentAlias = new Random().Next(1000000000, int.MaxValue).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetAccountsByAlias(channelId, unrecognizedEnrollmentAlias));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando se intenta obtener las cuentas usando un canal que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsByAliasUnrecognizedChannelThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string unrecognizedChannelId = Guid.NewGuid().ToString();
            string enrollmentAlias = "52080323";
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetAccountsByAlias(unrecognizedChannelId, enrollmentAlias));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
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
            const string AccountTypesPattern = "80|81|82|83|84";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (BalanceInfo balance in balances)
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
        public void GetBalancesUnrecognizedUserResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string unrecognizedDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string accountId = new Random().Next(99, 9999).ToString();
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(fixedDocType, unrecognizedDocNumber, accountId);
            CollectionAssert.IsEmpty(balances);
        }

        /// <summary>
        /// Obtener los saldos de una cuenta por el alias de registro de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesByAliasRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            IList<AccountInfo> accounts = client.Inquiries.GetAccountsByAlias(channelId, enrollmentAlias);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            IList<BalanceInfo> balances = client.Inquiries.GetBalancesByAlias(channelId, enrollmentAlias, accountId);
            CollectionAssert.IsNotEmpty(balances);
            Assert.That(balances.Count, Is.EqualTo(5));
            const string AccountTypesPattern = "80|81|82|83|84";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (BalanceInfo balance in balances)
            {
                Assert.That(balance.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(balance.Number, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(balance.SourceAccountId, Is.Not.Empty);
                Assert.That(balance.TypeId, Is.Not.Null.And.Match(AccountTypesPattern));
                Assert.That(balance.TypeName, Is.Not.Empty);
            }
        }

        /// <summary>
        /// Se produce una excepción cuando se intenta obtener los saldos de una cuenta usando un alias de registro que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesByAliasUnrecognizedEnrollmentAliasThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string unrecognizedEnrollmentAlias = new Random().Next(1000000000, int.MaxValue).ToString();
            string accountId = new Random().Next(99, 9999).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetBalancesByAlias(channelId, unrecognizedEnrollmentAlias, accountId));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando se intenta obtener los saldos de una cuenta usando un canal que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesByAliasUnrecognizedChannelThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string unrecognizedChannelId = Guid.NewGuid().ToString();
            string enrollmentAlias = "52080323";
            string accountId = new Random().Next(99, 9999).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetBalancesByAlias(unrecognizedChannelId, enrollmentAlias, accountId));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
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
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);
            
            // Los movimientos más recientes realizados por todas las cuentas...
            IList<MiniStatementInfo> statements = client.Inquiries.GetStatements(docType, docNumber, accountId);
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
            IList<MiniStatementInfo> statementsByAccountType = client.Inquiries.GetStatements(docType, docNumber, accountId, accountTypeId);
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
        /// Obtener los saldos de una cuenta de un usuario que no existe produce una salida exitosa pero sin resultados.
        /// </summary>|
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUnrecognizedUserResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string unrecognizedDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string accountId = new Random().Next(999, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(fixedDocType, unrecognizedDocNumber, accountId);
            Assert.DoesNotThrow(() => GetStatements());
            CollectionAssert.IsEmpty(GetStatements());
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta que no existe produce una una salida exitosa pero sin resultados.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsUnrecognizedAccountResponseIsEmpty()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string fixedDocType = "CC";
            string unrecognizedDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string unrecognizedAccountId = new Random().Next(999, 9999).ToString();
            IList<MiniStatementInfo> GetStatements() => client.Inquiries.GetStatements(fixedDocType, unrecognizedDocNumber, unrecognizedAccountId);
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
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);

            string unrecognizedAccountTypeId = new Random().Next(999, 9999).ToString();
            IList<MiniStatementInfo> statements = client.Inquiries.GetStatements(docType, docNumber, accountId, unrecognizedAccountTypeId);
            CollectionAssert.IsEmpty(statements);
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta por el alias de registro de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsByAliasRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            IList<AccountInfo> accounts = client.Inquiries.GetAccountsByAlias(channelId, enrollmentAlias);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;
            Assert.IsNotEmpty(accountId);
            IList<BalanceInfo> balances = client.Inquiries.GetBalancesByAlias(channelId, enrollmentAlias, accountId);
            CollectionAssert.IsNotEmpty(balances);

            // Los movimientos más recientes realizados por la cuenta...
            IList<MiniStatementInfo> statements = client.Inquiries.GetStatementsByAlias(channelId, enrollmentAlias, accountId);
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
            IList<MiniStatementInfo> statementsByAccountType = client.Inquiries.GetStatementsByAlias(channelId, enrollmentAlias, accountId, accountTypeId);
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
        /// Se produce una excepción cuando se intenta obtener los movimientos de una cuenta usando un alias de registro que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsByAliasUnrecognizedEnrollmentAliasThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string unrecognizedEnrollmentAlias = new Random().Next(1000000000, int.MaxValue).ToString();
            string accountId = new Random().Next(999, 9999).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetStatementsByAlias(channelId, unrecognizedEnrollmentAlias, accountId));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando se intenta obtener los movimientos de una cuenta usando un canal que no existe.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsByAliasUnrecognizedChannelThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string unrecognizedChannelId = Guid.NewGuid().ToString();
            string enrollmentAlias = "52080323";
            string accountId = new Random().Next(999, 9999).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Inquiries.GetStatementsByAlias(unrecognizedChannelId, enrollmentAlias, accountId));
            Assert.That(exception.EventId, Is.EqualTo("15881"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se ha encontrado información de enrolamiento con los valores suministrados", exception.Message);
        }

        /// <summary>
        /// Obtener las cuentas de los proveedores de datos conocidos.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsRecognizedDataProvidersRequestWorks()
        {
            // Se habilitan los proveedores conocidos para la aplicación...
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP|Bancor");

            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            Assert.That(accounts.Count, Is.EqualTo(2));

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (AccountInfo account in accounts)
            {
                Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                Assert.That(account.Name, Is.Not.Empty);
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

            // Se reestablece la aplicación para usar el proveedor de datos predeterminando para pruebas...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }

        /// <summary>
        /// Obtener las cuentas de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsSafelyRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountSafeInfo> inquiryResults = client.Inquiries.GetAccountsSafely(docType, docNumber);
            CollectionAssert.IsNotEmpty(inquiryResults);
            Assert.That(inquiryResults.Count, Is.EqualTo(1));

            const string AccountIdPattern = @"^[\d]*$";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (AccountSafeInfo inquiryResult in inquiryResults)
            {
                CollectionAssert.IsNotEmpty(inquiryResult.Data);
                Assert.IsNotEmpty(inquiryResult.Reason);
                Assert.That(inquiryResult.Status, Is.EqualTo(SubsystemStatus.Available));
                Assert.That(inquiryResult.Subsystem, Is.EqualTo(Subsystem.Tup));

                foreach (AccountInfo account in inquiryResult.Data)
                {
                    Assert.That(account.Balance, Is.AssignableTo(typeof(decimal)));
                    Assert.That(account.Id, Is.Not.Null.And.Match(AccountIdPattern));
                    Assert.That(account.SourceAccountId, Is.Not.Null.And.Match(AccountIdPattern));
                    Assert.That(account.MaskedPan, Is.Not.Null.And.Match(AccountNumberPattern));
                    Assert.That(account.Name, Is.Not.Empty);
                    CollectionAssert.IsNotEmpty(account.Properties);
                }
            }
        }

        /// <summary>
        /// Obtener las cuentas de los proveedores de datos conocidos.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsSafelyUnavailableRecognizedDataProviderRequestWorks()
        {
            // Se habilitan los proveedores conocidos para la aplicación...
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP|Bancor");

            // Se configura una conexión inválida a un proveedor de datos conocido para esperar que falle la consulta...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Broken");

            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountSafeInfo> inquiryResults = client.Inquiries.GetAccountsSafely(docType, docNumber);
            CollectionAssert.IsNotEmpty(inquiryResults);
            Assert.That(inquiryResults.Count, Is.EqualTo(2));

            // El proveedor de datos de TUP debe retornar un resultado por la consulta de cuentas...
            AccountSafeInfo tupInquiryResult = inquiryResults.First(info => info.Subsystem == Subsystem.Tup);
            CollectionAssert.IsNotEmpty(tupInquiryResult.Data);
            Assert.IsNotEmpty(tupInquiryResult.Reason);
            Assert.That(tupInquiryResult.Status, Is.EqualTo(SubsystemStatus.Available));

            // El proveedor de datos de BANCOR debe indicar que no está disponible para procesar la consulta requerida...
            AccountSafeInfo bancorInquiryResult = inquiryResults.First(info => info.Subsystem == Subsystem.Bancor);
            CollectionAssert.IsEmpty(bancorInquiryResult.Data);
            Assert.IsNotEmpty(bancorInquiryResult.Reason);
            Assert.That(bancorInquiryResult.Status, Is.EqualTo(SubsystemStatus.Unavailable));

            // Se reestablece la aplicación para usar el proveedor de datos predeterminando para pruebas...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            // Se reestablece las conexión valida al proveedor de datos conocido...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Bancor:Tests");
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesSafelyRequestWorks()
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

            IList<BalanceSafeInfo> inquiryResults = client.Inquiries.GetBalancesSafely(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(inquiryResults);
            Assert.That(inquiryResults.Count, Is.EqualTo(1));

            const string AccountTypesPattern = "80|81|82|83|84";
            const string AccountNumberPattern = @".*\d{4}";
            foreach (BalanceSafeInfo inquiryResult in inquiryResults)
            {
                CollectionAssert.IsNotEmpty(inquiryResult.Data);
                Assert.IsNotEmpty(inquiryResult.Reason);
                Assert.That(inquiryResult.Status, Is.EqualTo(SubsystemStatus.Available));
                Assert.That(inquiryResult.Subsystem, Is.EqualTo(Subsystem.Tup));
                Assert.That(inquiryResult.Data.Count, Is.EqualTo(5));

                foreach (BalanceInfo balance in inquiryResult.Data)
                {
                    Assert.That(balance.Balance, Is.AssignableTo(typeof(decimal)));
                    Assert.That(balance.Number, Is.Not.Null.And.Match(AccountNumberPattern));
                    Assert.That(balance.SourceAccountId, Is.Not.Empty);
                    Assert.That(balance.TypeId, Is.Not.Null.And.Match(AccountTypesPattern));
                    Assert.That(balance.TypeName, Is.Not.Empty);
                }
            }
        }

        /// <summary>
        /// Obtener los saldos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetBalancesSafelyUnavailableRecognizedDataProviderRequestWorks()
        {
            // Se habilitan los proveedores conocidos para la aplicación...
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP|Bancor");

            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            IList<AccountInfo> accounts = client.Inquiries.GetAccounts(docType, docNumber);
            CollectionAssert.IsNotEmpty(accounts);
            AccountInfo accountInfo = accounts.FirstOrDefault(account => account.Source == Subsystem.Tup);
            Assert.IsNotNull(accountInfo);
            string accountId = accountInfo.SourceAccountId;

            // Se configura una conexión inválida para el proveedor de datos de TUP para esperar que falle la consulta de saldos...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Broken");

            IList<BalanceSafeInfo> inquiryResults = client.Inquiries.GetBalancesSafely(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(inquiryResults);
            Assert.That(inquiryResults.Count, Is.EqualTo(2));

            // El proveedor de datos de TUP debe indicar que no está disponible para procesar la consulta requerida...
            BalanceSafeInfo tupInquiryResult = inquiryResults.First(info => info.Subsystem == Subsystem.Tup);
            CollectionAssert.IsEmpty(tupInquiryResult.Data);
            Assert.IsNotEmpty(tupInquiryResult.Reason);
            Assert.That(tupInquiryResult.Status, Is.EqualTo(SubsystemStatus.Unavailable));

            // El proveedor de datos de BANCOR debe indicar que no implementa la característica de saldos...
            BalanceSafeInfo bancorInquiryResult = inquiryResults.First(info => info.Subsystem == Subsystem.Bancor);
            CollectionAssert.IsEmpty(bancorInquiryResult.Data);
            Assert.IsNotEmpty(bancorInquiryResult.Reason);
            Assert.That(bancorInquiryResult.Status, Is.EqualTo(SubsystemStatus.MissingFeature));

            // Se reestablece la aplicación para usar el proveedor de datos predeterminando para pruebas...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");

            // Se reestablece las conexión valida al proveedor de datos de TUP...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Bifrost:Tests");
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsSafelyRequestWorks()
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
            Assert.IsNotEmpty(accountId);
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);

            void AssertStatementsSafeResults(IList<MiniStatementSafeInfo> statementInquiryResults)
            {
                CollectionAssert.IsNotEmpty(statementInquiryResults);
                Assert.That(statementInquiryResults.Count, Is.EqualTo(1));
                foreach (MiniStatementSafeInfo inquiryResult in statementInquiryResults)
                {
                    Assert.IsNotEmpty(inquiryResult.Reason);
                    Assert.That(inquiryResult.Status, Is.EqualTo(SubsystemStatus.Available));
                    Assert.That(inquiryResult.Subsystem, Is.EqualTo(Subsystem.Tup));
                    IList<MiniStatementInfo> statements = inquiryResult.Data;
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
                }
            }

            // Los movimientos más recientes realizados por la cuenta...
            IList<MiniStatementSafeInfo> inquiryResults = client.Inquiries.GetStatementsSafely(docType, docNumber, accountId);
            AssertStatementsSafeResults(inquiryResults);

            // Los movimientos más recientes realizados por la cuenta y el bolsillo específico...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            inquiryResults = client.Inquiries.GetStatementsSafely(docType, docNumber, accountId, accountTypeId);
            AssertStatementsSafeResults(inquiryResults);
        }

        /// <summary>
        /// Obtener los movimientos de una cuenta asociada a un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetStatementsSafelyUnavailableRecognizedDataProviderRequestWorks()
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
            Assert.IsNotEmpty(accountId);
            IList<BalanceInfo> balances = client.Inquiries.GetBalances(docType, docNumber, accountId);
            CollectionAssert.IsNotEmpty(balances);

            void AssertStatementsSafeResults(IList<MiniStatementSafeInfo> statementInquiryResults)
            {
                CollectionAssert.IsNotEmpty(statementInquiryResults);
                Assert.That(statementInquiryResults.Count, Is.EqualTo(1));
                foreach (MiniStatementSafeInfo inquiryResult in statementInquiryResults)
                {
                    CollectionAssert.IsEmpty(inquiryResult.Data);
                    Assert.IsNotEmpty(inquiryResult.Reason);
                    Assert.That(inquiryResult.Subsystem, Is.EqualTo(Subsystem.Tup));
                    Assert.That(inquiryResult.Status, Is.EqualTo(SubsystemStatus.Unavailable));
                }
            }

            // Se configura una conexión inválida para el proveedor de datos de TUP para esperar que falle la consulta de saldos...
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Broken");

            // Los movimientos más recientes realizados por la cuenta...
            IList<MiniStatementSafeInfo> inquiryResults = client.Inquiries.GetStatementsSafely(docType, docNumber, accountId);
            AssertStatementsSafeResults(inquiryResults);

            // Los movimientos más recientes realizados por la cuenta y el bolsillo específico...
            string accountTypeId = balances.First().TypeId;
            Assert.IsNotEmpty(accountTypeId);
            inquiryResults = client.Inquiries.GetStatementsSafely(docType, docNumber, accountId, accountTypeId);
            AssertStatementsSafeResults(inquiryResults);

            // Se reestablece las conexión valida al proveedor de datos de TUP...
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Bifrost:Tests");
        }
    }
}