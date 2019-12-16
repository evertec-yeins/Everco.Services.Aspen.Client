// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Fluent;
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
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        public IAutonomousApp GetAutonomousClient() =>
            AutonomousApp.Initialize()
                .RoutingTo(ServiceLocator.Instance.DefaultEndpoint)
                .WithIdentity(ServiceLocator.Instance.DefaultIdentity)
                .Authenticate()
                .GetClient();
    }
}