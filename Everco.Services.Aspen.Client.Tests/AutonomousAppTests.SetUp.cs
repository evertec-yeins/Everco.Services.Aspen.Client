// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.SetUp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-02 09:15 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Identities;
    using Identity;
    using NUnit.Framework;

    public enum ChannelTarget
    {
        ToInternalServerError = 0,
        ToUnrecognizedStatusCode = 1,
        ToCustomStatusCode = 2
    }

    /// <summary>
    /// Implementa atributos de configuración para ejecutar instrucciones en combinación con las pruebas automáticas.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán después de llamar a cada unidad de prueba.
        /// </summary>
        [TearDown]
        public void RunAfterTest()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "LocalTokenProvider");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:TokenProvider:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:Channel", "{Bifrost:Channel}");
        }

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán antes de llamar a cada unidad de prueba.
        /// </summary>
        [SetUp]
        public void RunBeforeTest()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán una única vez, antes de llamar al conjunto de pruebas implementadas.
        /// </summary>
        [OneTimeSetUp]
        public void RunBeforeTestFixture()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Bifrost:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Bancor:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Kraken:ConnectionStringName", "RabbitMQ:Kraken:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:TokenProvider:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "TUP:InquiryProvider", "Database");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:AuthProviderKey", "DbAuthProvider");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "LocalTokenProvider");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:SupportsChannels", "False");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:NullifyErrorBehavior", "SilentlyContinue");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:Channel", "{Bifrost:Channel}");
        }

        private static void SetChannelForRemoteTokenProvider(ChannelTarget target)
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string value = string.Empty;
            switch (target)
            {
                case ChannelTarget.ToInternalServerError:
                    value = "DEMOINTSERERR0000000";
                    break;

                case ChannelTarget.ToUnrecognizedStatusCode:
                    value = "DEMOINVRESCOD0000000";
                    break;

                case ChannelTarget.ToCustomStatusCode:
                    value = "DEMOCUSTOMCOD0000000";
                    break;
            }

            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:Channel", value);
        }

        private static void UseBrokenConnection()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "RemoteTokenProvider:ConnectionStringName", "RabbitMQ:Broken:Tests");
        }

        private static void UseRemoteTokenProvider()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "RemoteTokenProvider");
        }

        private static void UseUnrecognizedTokenProvider()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "IOC:TokenProviderKey", "UnrecognizedTokenProviderName");
        }
    }
}