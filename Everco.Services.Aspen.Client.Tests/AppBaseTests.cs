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
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public abstract class AppBaseTests
    {
        /// <summary>
        /// Obtiene un cliente para a partir de la aplicación autónoma de pruebas, omitiendo los valores almacenados en memoria.
        /// </summary>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        public virtual IAutonomousApp GetAutonomousClient() =>
            AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Master)
                .AuthenticateNoCache()
                .GetClient();

        /// <summary>
        /// Obtiene un cliente para a partir de la aplicación delegada de pruebas, omitiendo los valores almacenados en memoria. 
        /// </summary>
        /// <returns>Instancia de <see cref="IDelegatedApp"/> para interactuar con el servicio.</returns>
        public virtual IDelegatedApp GetDelegatedClient() =>
            DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .AuthenticateNoCache(RecognizedUserIdentity.Master)
                .GetClient();

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán al finalizar cada unidad de prueba.
        /// </summary>
        [TearDown]
        public virtual void RunAfterTest()
        {
        }

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán una única vez, al finalizar el conjunto de pruebas implementadas.
        /// </summary>
        [OneTimeTearDown]
        public virtual void RunAfterTestFixture()
        {
        }

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán antes de llamar a cada unidad de prueba.
        /// </summary>
        [SetUp]
        public virtual void RunBeforeTest()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Proporciona un conjunto común de instrucciones que se ejecutarán una única vez, antes de llamar al conjunto de pruebas implementadas.
        /// </summary>
        [OneTimeSetUp]
        public virtual void RunBeforeTestFixture()
        {
        }
    }
}