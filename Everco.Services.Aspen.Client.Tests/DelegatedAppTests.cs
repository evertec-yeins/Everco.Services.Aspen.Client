// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de delegada.
    /// </summary>
    public partial class DelegatedAppTests : AppBaseTests
    {
        /// <summary>
        /// Los procesos que fueron iniciados por cada servicio de prueba.
        /// </summary>
        private readonly DummyServices dummyServices = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AutonomousAppTests" />.
        /// </summary>
        public DelegatedAppTests()
        {
            this.dummyServices = new DummyServices().StartBifrostService();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }

        /// <summary>
        /// Finaliza una instancia de la clase <see cref="AutonomousAppTests" />.
        /// </summary>
        ~DelegatedAppTests() => this.dummyServices.Dispose();
    }
}