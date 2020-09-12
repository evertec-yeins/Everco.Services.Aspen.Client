// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.TokenModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 17:22 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Entities;
    using Entities.Delegated;
    using Fluent;
    using Identities;
    using Identity;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las entidades de información relacionadas con parametrización del sistema para una aplicación de alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtener la lista de canales (usando el proveedor de tokens local) para los que se pueden generar tokens o claves transaccionales funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenChannelsWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
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
            IDelegatedApp client = this.GetDelegatedClient();
            UseRemoteTokenProvider();
            IList<TokenChannelInfo> tokenChannels = client.Token.GetChannels();
            CollectionAssert.IsEmpty(tokenChannels);
            CollectionAssert.IsEmpty(client.Token.GetChannelsAsync().Result);
            Printer.Print(tokenChannels, "TokenChannels");
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Enviar un token transaccional al usuario autenticado funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            DeliveredTokenCustomData customData = null;
            Assert.DoesNotThrow(() => customData = client.Token.SendToken());
            Assert.NotNull(customData);
            Printer.Print(customData, "DeliveredTokenCustomData");

            if (customData.DebugMode)
            {
                Assert.IsNotEmpty(customData.Token);
            }
        }

        /// <summary>
        /// Enviar un token transaccional al usuario autenticado funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenFromRemoteProviderWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            UseRemoteTokenProvider();
            DeliveredTokenCustomData customData = null;
            Assert.DoesNotThrow(() => customData = client.Token.SendToken());
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
            IDelegatedApp client = this.GetDelegatedClient();
            UseRemoteTokenProvider();
            UseBrokenConnection();
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken());
            Assert.That(exception.EventId, Is.EqualTo("995058"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error con el sistema para la generación de tokens transaccionales", exception.Message);
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Generar un token transaccional al usuario autenticado funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GenerateTokenWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string pinNumber = "141414";
            TokenResponseInfo tokenResponseInfo = null;
            Assert.DoesNotThrow(() => tokenResponseInfo = client.Token.GenerateToken(pinNumber));
            Printer.Print(tokenResponseInfo, "TokenResponseInfo");
            Assert.NotNull(tokenResponseInfo);
            Assert.IsNotEmpty(tokenResponseInfo.Token);
            Assert.That(tokenResponseInfo.ExpirationMinutes, Is.GreaterThan(0));
            Assert.That(tokenResponseInfo.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));
        }

        /// <summary>
        /// Generar un token transaccional al usuario autenticado con un número de pin inválido no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GenerateTokenWithInvalidPinNumberThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string invalidPinNumber = new Random().Next(99999, 999999).ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.GenerateToken(invalidPinNumber));
            Assert.That(exception.EventId, Is.EqualTo("15862"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Pin de usuario/app no es valido. No corresponde con el registrado.", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción al solicitar la generación de un token transaccional para el usuario autenticado cuando el proveedor remoto no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GenerateWhenRemoteProviderBrokenThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            UseRemoteTokenProvider();
            UseBrokenConnection();
            string pinNumber = "141414";
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.GenerateToken(pinNumber));
            Assert.That(exception.EventId, Is.EqualTo("995060"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error con el sistema para la generación de tokens transaccionales", exception.Message);
            UseProvidedConnection();
            UseLocalTokenProvider();
        }

        /// <summary>
        /// La solicitud falla si no se reconoce el nombre del proveedor de token transaccionales establecido para la aplicación.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void UnrecognizedTokenProviderNameThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            UseUnrecognizedTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("15898"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
            StringAssert.IsMatch("No se reconoce el nombre el proveedor de token transaccionales configurado para la aplicación", exception.Message);
            UseLocalTokenProvider();
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario usando un canal nulo o vacío genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void EmptyOrWhitespaceChannelWhenGenerateTokenThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string pinNumber = "141414";
            List<string> invalidChannels = new List<string>()
            {
                string.Empty,
                "     "
            };

            foreach (string invalidChannel in invalidChannels)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.GenerateToken(pinNumber, channelKey: invalidChannel));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'ChannelKey' no puede ser vacío", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario usando un canal desconocido genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void UnrecognizedChannelWhenGenerateTokenThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            string pinNumber = "141414";
            List<string> unrecognizedChannels = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**"
            };

            foreach (string unrecognizedChannel in unrecognizedChannels)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.GenerateToken(pinNumber, channelKey: unrecognizedChannel));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("No existe un canal con el código proporcionado", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario usando un canal desconocido funciona,
        /// cuando no se soportan los canales usando el proveedor remoto de tokens.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void UnrecognizedChannelWhenGenerateTokenFromRemoteProviderWorks()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            UseRemoteTokenProvider();

            string pinNumber = "141414";
            List<string> unrecognizedChannels = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**"
            };

            foreach (string unrecognizedChannel in unrecognizedChannels)
            {
                TokenResponseInfo tokenResponseInfo = null;
                Assert.DoesNotThrow(() => tokenResponseInfo = client.Token.GenerateToken(pinNumber, channelKey: unrecognizedChannel));
                Printer.Print(tokenResponseInfo, "TokenResponseInfo");
                Assert.NotNull(tokenResponseInfo);
                Assert.IsNotEmpty(tokenResponseInfo.Token);
                Assert.That(tokenResponseInfo.ExpirationMinutes, Is.GreaterThan(0));
                Assert.That(tokenResponseInfo.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));
            }

            UseLocalTokenProvider();
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario usando un canal desconocido genera una respuesta inválida,
        /// cuando se soportan los canales usando el proveedor remoto de tokens.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void UnrecognizedChannelWhenGenerateTokenFromRemoteProviderThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            UseRemoteTokenProvider();
            SupportsChannels();

            string pinNumber = "141414";
            List<string> unrecognizedChannels = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**"
            };

            foreach (string unrecognizedChannel in unrecognizedChannels)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.GenerateToken(pinNumber, channelKey: unrecognizedChannel));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("No existe un canal con el código proporcionado", exception.Message);
            }

            UseLocalTokenProvider();
            NotSupportsChannels();
        }

        private static void SupportsChannels()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:SupportsChannels", "True");
        }

        private static void NotSupportsChannels()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:SupportsChannels", "False");
        }

        private static void UseRemoteTokenProvider()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "RemoteTokenProvider");
        }

        private static void UseLocalTokenProvider()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "LocalTokenProvider");
        }

        private static void UseUnrecognizedTokenProvider()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "UnrecognizedTokenProviderName");
        }

        private static void UseBrokenConnection()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:Broken:Tests");
        }

        private static void UseProvidedConnection()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:TokenProvider:Tests");
        }
    }
}