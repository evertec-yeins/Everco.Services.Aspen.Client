// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Fluent;
    using Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obtiene un cliente para a partir de la aplicación autónoma de pruebas, omitiendo los valores almacenados en memoria.
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        public IAutonomousApp GetAutonomousClient(CachePolicy cachePolicy = CachePolicy.BypassCache) =>
            AutonomousApp.Initialize(cachePolicy)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Master)
                .Authenticate()
                .GetClient();
    }
}