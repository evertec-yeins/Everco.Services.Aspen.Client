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
    using System.Text.RegularExpressions;
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
            Assert.That(exception.EventId, Is.EqualTo("355003"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error general con el sistema externo que administra los tokens o claves transaccionales", exception.Message);
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
        /// Obtener la lista de canales (usando el proveedor de tokens remoto) para los que se pueden generar tokens o claves transaccionales funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void GetTokenChannelsFromRemoteProviderWorks()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();
            IList<TokenChannelInfo> tokenChannels = client.Token.GetChannels();
            CollectionAssert.IsNotEmpty(tokenChannels);
            CollectionAssert.IsNotEmpty(client.Token.GetChannelsAsync().Result);
            Printer.Print(tokenChannels, "TokenChannels");
        }

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
        /// Redimir un token transaccional que y expiró no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        [Category("Modules.Token.LocalProvider")]
        public void RedeemTokenExpiredThrows()
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
            string token = customData.Token;
            int amount = new Random().Next(10000, 50000);
            string appKey = AutonomousAppIdentity.Master.ApiKey;
            TestContext.CurrentContext.DatabaseHelper().EnsureExpireToken(docType, docNumber, appKey);

            AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("El token ya expiró", exception.Message);
        }

        /// <summary>
        /// Redimir un token aleatorio no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        [Category("Modules.Token.RemoteProvider")]
        public void RedeemTokenInvalidFromRemoteProviderThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            UseRemoteTokenProvider();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            int amount = new Random().Next(10000, 50000);

            string token = "000000";
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se encontró token con los valores proporcionados", exception.Message);

            token = new Random().Next(100000, 999999).ToString();
            exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Token inválido", exception.Message);

            token = "000001";
            exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355008"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            StringAssert.IsMatch("Una respuesta con código de error interno", exception.Message);

            token = "000002";
            exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Una respuesta con código HTTP desconocido", exception.Message);

            token = "000003";
            exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355010"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch("Una respuesta con código HTTP válido", exception.Message);
        }

        /// <summary>
        /// Redimir un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        [Category("Modules.Token.RemoteProvider")]

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
        }
        /// <summary>
        /// Redimir un token transaccional invalido no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        [Category("Modules.Token.LocalProvider")]
        public void RedeemTokenInvalidThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = TestContext.CurrentContext.Random.GetString(6, "0123456789");
            int amount = new Random().Next(10000, 50000);
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("355006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            StringAssert.IsMatch("No se encontró un token con los valores proporcionados", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción al validar un token transaccional cuando el proveedor remoto no funciona.
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
            Assert.That(exception.EventId, Is.EqualTo("355009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error general con el sistema externo que administra los tokens o claves transaccionales", exception.Message);
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithInvalidAmountThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();

            List<int> invalidAmounts = new List<int>()
            {
                -1,
                -10000
            };

            foreach (int invalidAmount in invalidAmounts)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, invalidAmount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Amount' debe ser un valor numérico mayor que cero", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithInvalidDocNumberFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);

            List<string> invalidDocNumbers = new List<string>()
            {
                "X",
                "XXXXXXX",
                "123456XXXX",
                TestContext.CurrentContext.Random.GetString(30),
                TestContext.CurrentContext.Random.GetString(30, "0123456789")
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, invalidDocNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("355005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("El número de documento es inválido", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithInvalidDocNumberThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);

            List<string> invalidDocNumbers = new List<string>()
            {
                "X",
                "XXXXXXX",
                "123456XXXX",
                TestContext.CurrentContext.Random.GetString(30),
                TestContext.CurrentContext.Random.GetString(30, "0123456789")
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, invalidDocNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithInvalidMetadataThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);

            List<string> invalidValues = new List<string>()
            {
                TestContext.CurrentContext.Random.GetString(51),
                TestContext.CurrentContext.Random.GetString(100, "0123456789")
            };

            foreach (string invalidMetadata in invalidValues)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount, invalidMetadata));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Metadata' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithNullOrEmptyDocNumberThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);

            List<string> invalidDocNumbers = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, invalidDocNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla con un tipo de documento nulo o vacío.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithNullOrEmptyDocTypeFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            List<string> invalidDocTypes = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocType in invalidDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(invalidDocType, docNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla con un tipo de documento nulo o vacío.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithNullOrEmptyDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            List<string> invalidDocTypes = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocType in invalidDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(invalidDocType, docNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithNullOrEmptyMetadataThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);

            List<string> invalidValues = new List<string>()
            {
                "",
                "   "
            };

            foreach (string invalidMetadata in invalidValues)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount, invalidMetadata));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Metadata' no puede ser vacío", exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla si no se reconoce el tipo de documento desconocido del usuario.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithUnrecognizedDocTypeFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();

            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            List<string> unrecognizedDocTypes = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**",
                "@",
                "CCC",
                "CCCCXXXX",
                "00000000"
            };

            foreach (string unrecognizedDocType in unrecognizedDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(unrecognizedDocType, docNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                string pattern = $"'{Regex.Escape(unrecognizedDocType)}' no se reconoce como un tipo de identificación";
                StringAssert.IsMatch(pattern, exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla si no se reconoce el tipo de documento desconocido del usuario.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithUnrecognizedDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            List<string> unrecognizedDocTypes = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**",
                "@",
                "CCC",
                "CCCCXXXX",
                "00000000"
            };

            foreach (string unrecognizedDocType in unrecognizedDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(unrecognizedDocType, docNumber, token, amount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                string pattern = $"'{Regex.Escape(unrecognizedDocType)}' no se reconoce como un tipo de identificación";
                StringAssert.IsMatch(pattern, exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla si no se reconoce el nombre del proveedor de token transaccionales establecido para la aplicación.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void RedeemTokenWithUnrecognizedTokenProviderNameThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseUnrecognizedTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            string token = new Random().Next(99999, 999999).ToString();
            int amount = new Random().Next(10000, 50000);
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.RedeemToken(docType, docNumber, token, amount));
            Assert.That(exception.EventId, Is.EqualTo("15898"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
            StringAssert.IsMatch("No se reconoce el nombre el proveedor de token transaccionales configurado para la aplicación", exception.Message);
        }

        /// <summary>
        /// Redimir un token transaccional emitido funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        [Category("Modules.Token.LocalProvider")]
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
        /// Enviar un token transaccional a un usuario cuando usando el proveedor de tokens remoto y genera un respuesta con código de error interno (500).
        /// </summary>
        /// <remarks>
        /// Esta prueba es solo para comprobar las respuestas HTTP del servicio ASPEN mediante casos identificados que se imitan con un microservicio con respuestas ficticias.
        /// En ningún caso estas respuestas corresponden con el microservicio real.
        /// </remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenFromRemoteProviderWhenResponseWithInternalErrorWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            SetChannelForRemoteTokenProvider(ChannelTarget.ToInternalServerError);
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("355002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            StringAssert.IsMatch("Una respuesta con código de error interno", exception.Message);
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario cuando usando el proveedor de tokens remoto y genera un respuesta con un código conocido.
        /// </summary>
        /// <remarks>
        /// Esta prueba es solo para comprobar las respuestas HTTP del servicio ASPEN mediante casos identificados que se imitan con un microservicio con respuestas ficticias.
        /// En ningún caso estas respuestas corresponden con el microservicio real.
        /// </remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenFromRemoteProviderWhenResponseWithRecognizedStatusCodeWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            SetChannelForRemoteTokenProvider(ChannelTarget.ToCustomStatusCode);
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("355004"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            StringAssert.IsMatch("Una respuesta con código HTTP válido", exception.Message);
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario cuando usando el proveedor de tokens remoto y genera un respuesta con un código desconocido debe gerenar a cambio un bad gateway (502)
        /// </summary>
        /// <remarks>
        /// Esta prueba es solo para comprobar las respuestas HTTP del servicio ASPEN mediante casos identificados que se imitan con un microservicio con respuestas ficticias.
        /// En ningún caso estas respuestas corresponden con el microservicio real.
        /// </remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenFromRemoteProviderWhenResponseWithUnrecognizedStatusCodeWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            SetChannelForRemoteTokenProvider(ChannelTarget.ToUnrecognizedStatusCode);
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("355003"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Una respuesta con código HTTP desconocido", exception.Message);
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
            Assert.That(exception.EventId, Is.EqualTo("355003"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
            StringAssert.IsMatch("Se ha presentado un error general con el sistema externo que administra los tokens o claves transaccionales", exception.Message);
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario cuando se establece un tipo de cuenta inválido al proveedor de tokens remotos no funciona.
        /// </summary>
        /// <remarks>
        /// Esta prueba es solo para comprobar las respuestas HTTP del servicio ASPEN mediante casos identificados que se imitan con un microservicio con respuestas ficticias.
        /// En ningún caso estas respuestas corresponden con el microservicio real.
        /// </remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidAccountTypeFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidAccountTypes = new List<string>()
            {
                "   ",
                "X1",
                "XX",
                TestContext.CurrentContext.Random.GetString(1, "0123456789"),
                TestContext.CurrentContext.Random.GetString(3, "0123456789")
            };

            foreach (string invalidAccountType in invalidAccountTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, invalidAccountType));
                Assert.That(exception.EventId, Is.EqualTo("355001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("El tipo de cuenta es inválido", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidAccountTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string docType = userIdentity.DocType;
            List<string> invalidAccountTypes = new List<string>()
            {
                "X",
                "XXX",
                "123456XXXX",
                TestContext.CurrentContext.Random.GetString(30),
                TestContext.CurrentContext.Random.GetString(30, "0123456789")
            };

            foreach (string invalidAccountType in invalidAccountTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, accountType: invalidAccountType));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'AccountType' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidAmountThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string docType = userIdentity.DocType;
            List<int> invalidAmounts = new List<int>()
            {
                -1,
                -10000
            };

            foreach (int invalidAmount in invalidAmounts)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, amount: invalidAmount));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Amount' debe ser un valor numérico mayor que cero", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario cuando se establece un número de documento inválido al proveedor de tokens remoto no funciona.
        /// </summary>
        /// <remarks>
        /// Esta prueba es solo para comprobar las respuestas HTTP del servicio ASPEN mediante casos identificados que se imitan con un microservicio con respuestas ficticias.
        /// En ningún caso estas respuestas corresponden con el microservicio real.
        /// </remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidDocNumberFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            List<string> invalidDocNumbers = new List<string>()
            {
                "X",
                "XXXXXXX",
                "123456XXXX",
                TestContext.CurrentContext.Random.GetString(30),
                TestContext.CurrentContext.Random.GetString(30, "0123456789")
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, invalidDocNumber));
                Assert.That(exception.EventId, Is.EqualTo("355001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("El número de documento es inválido", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidDocNumberThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            List<string> invalidDocNumbers = new List<string>()
            {
                "X",
                "XXXXXXX",
                "123456XXXX",
                TestContext.CurrentContext.Random.GetString(30),
                TestContext.CurrentContext.Random.GetString(30, "0123456789")
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, invalidDocNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional a un usuario cuando se establece un tipo de documento inválido al proveedor de tokens remotos no funciona.
        /// </summary>
        /// <remarks>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidDocTypeFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidDocTypes = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**",
                "@",
                "CCC",
                "CCCCXXXX",
                "00000000"
            };

            foreach (string invalidDocType in invalidDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(invalidDocType, docNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                string pattern = $"'{Regex.Escape(invalidDocType)}' no se reconoce como un tipo de identificación";
                StringAssert.IsMatch(pattern, exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithInvalidMetadataThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidValues = new List<string>()
            {
                TestContext.CurrentContext.Random.GetString(51),
                TestContext.CurrentContext.Random.GetString(100, "0123456789")
            };

            foreach (string invalidMetadata in invalidValues)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, metadata: invalidMetadata));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Metadata' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyAccountTypeThenUseDefaultValueFromRemoteProviderWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseRemoteTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidAccountTypes = new List<string>()
            {
                null,
                ""
            };

            foreach (string invalidAccountType in invalidAccountTypes)
            {
                DeliveredTokenCustomData deliveredTokenInfo = null;
                Assert.DoesNotThrow(() => deliveredTokenInfo = client.Token.SendToken(docType, docNumber, invalidAccountType));
                Assert.NotNull(deliveredTokenInfo);
                Printer.Print(deliveredTokenInfo, "DeliveredTokenCustomData");

                if (deliveredTokenInfo.DebugMode)
                {
                    Assert.IsNotEmpty(deliveredTokenInfo.Token);
                }
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyAccountTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            string docType = userIdentity.DocType;
            List<string> invalidAccountTypes = new List<string>()
            {
                "",
                "   "
            };

            foreach (string invalidAccountType in invalidAccountTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, accountType: invalidAccountType));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'AccountType' no puede ser vacío", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyDocNumberThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            List<string> invalidDocNumbers = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocNumber in invalidDocNumbers)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, invalidDocNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional con un tipo de documento nulo o vacío a un usuario no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyDocTypeFromRemoteProviderThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            UseRemoteTokenProvider();

            List<string> invalidDocTypes = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocType in invalidDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(invalidDocType, docNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional con un tipo de documento nulo o vacío a un usuario no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidDocTypes = new List<string>()
            {
                null,
                "",
                "   "
            };

            foreach (string invalidDocType in invalidDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(invalidDocType, docNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithNullOrEmptyMetadataThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            List<string> invalidValues = new List<string>()
            {
                "",
                "   "
            };

            foreach (string invalidMetadata in invalidValues)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, metadata: invalidMetadata));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Metadata' no puede ser vacío", exception.Message);
            }
        }

        /// <summary>
        /// Enviar un token transaccional con un tipo de documento desconocido a un usuario no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithUnrecognizedDocTypeThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docNumber = userIdentity.DocNumber;
            List<string> unrecognizedDocTypes = new List<string>()
            {
                "XX",
                "00",
                "X0",
                "**",
                "@",
                "CCC",
                "CCCCXXXX",
                "00000000"
            };

            foreach (string unrecognizedDocType in unrecognizedDocTypes)
            {
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(unrecognizedDocType, docNumber));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no se reconoce como un tipo de identificación", exception.Message);
            }
        }

        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithUnrecognizedTagThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;

            List<string> invalidKeys = new List<string>()
            {
                "XXXXXX",
                "Terminal_Id",
                "Card_Acceptor_Id",
                "Customer_Group",
                "Pan_",
                TestContext.CurrentContext.Random.GetString(10),
                TestContext.CurrentContext.Random.GetString(10, "0123456789")
            };

            foreach (string invalidKey in invalidKeys)
            {
                Dictionary<string, string> customData = new Dictionary<string, string>() {
                    { invalidKey, TestContext.CurrentContext.Random.GetString(10) }
                };
                AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber, tags: customData));
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                string pattern = $"'{invalidKey}' no se reconoce como un valor de tag admitido";
                StringAssert.IsMatch(pattern, exception.Message);
            }
        }

        /// <summary>
        /// La solicitud falla si no se reconoce el nombre del proveedor de token transaccionales establecido para la aplicación.
        /// </summary>
        [Test]
        [Category("Modules.Token")]
        public void SendTokenWithUnrecognizedTokenProviderNameThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            UseUnrecognizedTokenProvider();
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            string docType = userIdentity.DocType;
            string docNumber = userIdentity.DocNumber;
            AspenException exception = Assert.Throws<AspenException>(() => client.Token.SendToken(docType, docNumber));
            Assert.That(exception.EventId, Is.EqualTo("15898"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
            StringAssert.IsMatch("No se reconoce el nombre el proveedor de token transaccionales configurado para la aplicación", exception.Message);
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
    }
}