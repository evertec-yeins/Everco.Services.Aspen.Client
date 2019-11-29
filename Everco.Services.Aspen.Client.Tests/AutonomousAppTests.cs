// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Identity;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests : AppBaseTests
    {
        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán una única vez, antes de llamar al conjunto de pruebas implementadas.
        /// </summary>
        public override void RunBeforeTestFixture()
        {
            base.RunBeforeTestFixture();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Bifrost:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Bancor:Tests");
            TestContext.CurrentContext.DatabaseHelper().SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }
    }
}