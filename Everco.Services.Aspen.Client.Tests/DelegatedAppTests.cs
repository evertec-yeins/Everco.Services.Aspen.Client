// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Providers;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtiene la instancia de un cliente para una aplicación con alcance de delegada.
        /// </summary>
        private static IDelegatedApp Client => DelegatedApp.Initialize()
                                                    .RoutingTo(EnvironmentEndpointProvider.Default)
                                                    .WithIdentity(DelegatedAppIdentity.Default)
                                                    .Authenticate(UserIdentity.Default)
                                                    .GetClient();

        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán antes de llamar a cada método de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación omitiendo cualquier valor en la cache.
        /// </summary>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        private static IDelegatedApp AuthenticateNoCache() =>
            DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();
    }
}