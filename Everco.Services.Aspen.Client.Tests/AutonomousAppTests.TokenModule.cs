// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.TokenModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 17:15 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Entities;
    using Entities.Autonomous;
    using Fluent;
    using Identities;
    using Identity;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a entidades de administración para una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obtener la lista de canales para los que se pueden generar tokens o claves transaccionales funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenChannelsWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            IList<TokenChannelInfo> tokenChannels = client.Token.GetChannels();
            CollectionAssert.IsNotEmpty(tokenChannels);
            CollectionAssert.IsNotEmpty(client.Token.GetChannelsAsync().Result);
            Printer.Print(tokenChannels, "TokenChannels");
        }

        /// <summary>
        /// Obtener la lista de canales (usando el proveedor de tokens remoto) para los que se pueden generar tokens o claves transaccionales funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenChannelsFromRemoteProviderWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();
            IList<TokenChannelInfo> tokenChannels = client.Token.GetChannels();
            CollectionAssert.IsEmpty(tokenChannels);
            CollectionAssert.IsEmpty(client.Token.GetChannelsAsync().Result);
            Printer.Print(tokenChannels, "TokenChannels");
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = null;
            Assert.DoesNotThrow(() => customData = client.Token.SendToken(docType, docNumber));
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
            }
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenFromRemoteProviderWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = null;
            Assert.DoesNotThrow(() => customData = client.Token.SendToken(docType, docNumber));
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
            }

            UseLocalTokenProvider();
        }

        /// <summary>
        /// Se produce una excepción al enviar un token transaccional al usuario autenticado cuando el proveedor remoto no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWhenRemoteProviderBrokenThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            UseRemoteTokenProvider();
            UseBrokenConnection();
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("995058"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error con el sistema para la generación de tokens transaccionales", exception.Message);
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Redimir un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = client.Token.SendToken(docType, docNumber);
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (!customData.DebugMode)
            {
                return;
            }

            Assert.IsNotEmpty(customData.Token);
            TokenRedeemedInfo tokenRedeemedInfo = null;
            string token = customData.Token;
            int amount = new Random().Next(10000, 50000);
            Assert.DoesNotThrow(() => tokenRedeemedInfo = client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.NotNull(tokenRedeemedInfo);
            Printer.Print(tokenRedeemedInfo, "TokenRedeemedInfo");
            Assert.IsNull(tokenRedeemedInfo.Amount);
            Assert.IsNull(tokenRedeemedInfo.AccountType);
        }

        /// <summary>
        /// Redimir un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenFromRemoteProviderWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = client.Token.SendToken(docType, docNumber);
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
                TokenRedeemedInfo tokenRedeemedInfo = null;
                string token = customData.Token;
                int amount = new Random().Next(10000, 50000);
                Assert.DoesNotThrow(() => tokenRedeemedInfo = client.Token.RedeemToken(docType, docNumber, token, amount));
                Assert.NotNull(tokenRedeemedInfo);
                Printer.Print(tokenRedeemedInfo, "TokenRedeemedInfo");
                Assert.IsNull(tokenRedeemedInfo.Amount);
                Assert.IsNull(tokenRedeemedInfo.AccountType);
            }
            
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Se produce una excepción al enviar un token transaccional al usuario autenticado cuando el proveedor remoto no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWhenRemoteProviderBrokenThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            UseRemoteTokenProvider();
            UseBrokenConnection();
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("995059"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error con el sistema para validación de tokens transaccionales", exception.Message);
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Obtener un token o clave transaccional en formato imagen (base64) a partir del alias de registro de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenByEnrollmentAliasWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            string imageBase64 = null;
            Assert.DoesNotThrow(() => imageBase64 = client.Token.GetTokenByAlias(channelId, enrollmentAlias));
            Assert.IsNotEmpty(imageBase64);
            Printer.Print(imageBase64, "TokenForEnrollmentAlias");
        }

        /// <summary>
        /// Obtener un token o clave transaccional en formato imagen (base64) a partir del alias de registro de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenByEnrollmentAliasFromRemoteProviderWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            string imageBase64 = null;
            Assert.DoesNotThrow(() => imageBase64 = client.Token.GetTokenByAlias(channelId, enrollmentAlias));
            Assert.IsNotEmpty(imageBase64);
            Printer.Print(imageBase64, "TokenForEnrollmentAlias");
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Se produce una excepción al obtener un token o clave transaccional en formato imagen (base64)
        /// a partir del alias de registro de un usuario  cuando el proveedor remoto no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenByEnrollmentAliasWhenRemoteProviderBrokenThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            string channelId = "5845EDE2-6593-4183-A2B5-280EA15F89E4";
            string enrollmentAlias = "52080323";
            UseRemoteTokenProvider();
            UseBrokenConnection();
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.GetTokenByAlias(channelId, enrollmentAlias));
            Assert.That(exception.EventId, Is.EqualTo("995061"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error con el sistema para la generación de tokens transaccionales", exception.Message);
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Cancelar un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void NullifyTokenWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = client.Token.SendToken(docType, docNumber);
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
                string token = customData.Token;
                int appId = TestContext.CurrentContext.DatabaseHelper().GetAppId(AutonomousAppIdentity.Master.ApiKey);
                Assert.DoesNotThrow(() => ((AutonomousApp)client.Token).NullifyToken(docType, docNumber, token, appId));
            }
        }

        /// <summary>
        /// Cancelar un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void NullifyTokenFromRemoteProviderWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            DeliveredTokenCustomData customData = client.Token.SendToken(docType, docNumber);
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
                string token = customData.Token;
                int appId = TestContext.CurrentContext.DatabaseHelper().GetAppId(AutonomousAppIdentity.Master.ApiKey);
                Assert.DoesNotThrow(() => ((AutonomousApp)client.Token).NullifyToken(docType, docNumber, token, appId));
            }
            
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Se produce una excepción al obtener un token o clave transaccional en formato imagen (base64)
        /// a partir del alias de registro de un usuario  cuando el proveedor remoto no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void NullifyTokenWhenRemoteProviderBrokenWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            UseBrokenConnection();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int appId = TestContext.CurrentContext.DatabaseHelper().GetAppId(AutonomousAppIdentity.Master.ApiKey);
            Assert.DoesNotThrow(() => ((AutonomousApp)client.Token).NullifyToken(docType, docNumber, token, appId));
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        private static void UseRemoteTokenProvider()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "RemoteTokenProvider");
        }

        private static void UseLocalTokenProvider()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "LocalTokenProvider");
        }

        private static void UseBrokenConnection()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:Broken:Tests");
        }

        private static void UseProvidedConnection()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:TokenProvider:Tests");
        }
    }
}