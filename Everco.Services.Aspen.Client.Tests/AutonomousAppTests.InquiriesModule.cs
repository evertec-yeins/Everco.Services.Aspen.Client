// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.InquiriesModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 15:06 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias de las consultas de los productos financieros del usuario.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obttener las cuentas de un usuario produce una salida válida.
        /// </summary>
        [Test]
        [Category("Modules.Inquiries")]
        public void GetAccountsRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            IUserIdentity userIdentity = UserIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Local");
            Assert.DoesNotThrow(() => client.Inquiries.GetAccounts(userIdentity.DocType, userIdentity.DocNumber));
        }
    }
}