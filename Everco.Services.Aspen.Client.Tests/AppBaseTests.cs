// -----------------------------------------------------------------------
// <copyright file="AppBaseTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-20 11:18 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Providers;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public abstract class AppBaseTests
    {
        /// <summary>
        /// Los procesos que fueron iniciados por cada servicio de prueba.
        /// </summary>
        private DummyServices dummyServices = null;

        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán una única vez, al finalizar el conjunto de pruebas implementadas.
        /// </summary>
        [OneTimeTearDown]
        public virtual void Cleanup()
        {
            this.dummyServices?.Dispose();
        }

        /// <summary>
        /// Obtiene un cliente para la aplicación autónoma de pruebas a partir de la solicitud de generación de un token de autenticación.
        /// </summary>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        public virtual IAutonomousApp GetAutonomousClient() =>
            AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Master)
                .AuthenticateNoCache()
                .GetClient();

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación omitiendo cualquier valor almacenado en memoria.
        /// </summary>
        /// <returns>Instancia de <see cref="IDelegatedApp"/> para interactuar con el servicio.</returns>
        public virtual IDelegatedApp GetDelegatedClient() =>
            DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .AuthenticateNoCache(RecognizedUserIdentity.Master)
                .GetClient();

        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán una única vez, antes de llamar al conjunto de pruebas implementadas.
        /// </summary>
        [OneTimeSetUp]
        public virtual void Init()
        {
            this.dummyServices = new DummyServices().StartBifrostService();
        }

        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán antes de llamar a cada método de prueba.
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            ServiceLocator.Instance.Reset();
        }
    }
}