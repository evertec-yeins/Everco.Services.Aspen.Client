// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
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
        /// Obtiene un cliente para a partir de la aplicación delegada de pruebas, omitiendo los valores almacenados en memoria. 
        /// </summary>
        /// <returns>Instancia de <see cref="IDelegatedApp"/> para interactuar con el servicio.</returns>
        public IDelegatedApp GetDelegatedClient() =>
            DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .AuthenticateNoCache(RecognizedUserIdentity.Master)
                .GetClient();
    }
}