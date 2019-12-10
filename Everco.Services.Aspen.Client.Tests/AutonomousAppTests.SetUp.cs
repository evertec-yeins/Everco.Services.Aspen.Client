// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.SetUp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-02 09:15 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Identity;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa atributos de configuración para ejecutar instrucciones en combinación con las pruebas automáticas.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
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
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }
    }
}