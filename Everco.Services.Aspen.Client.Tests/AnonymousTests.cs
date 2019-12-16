// -----------------------------------------------------------------------
// <copyright file="AnonymousTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-09 16:40 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Diagnostics;
    using Identity;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones anónimas del servicio.
    /// </summary>
    [TestFixture]
    public partial class AnonymousTests
    {

        [Test]
        public void ABC()
        {
            ServiceLocator.Instance.SetDefaultEndpoint(new RegistryEndpoint());
            //ServiceLocator.Instance.SetDefaultIdentity(new RegistryIdentity());
            Console.WriteLine(JsonConvert.SerializeObject(ServiceLocator.Instance.DefaultEndpoint, Formatting.Indented));
            //Console.WriteLine(JsonConvert.SerializeObject(ServiceLocator.Instance.DefaultIdentity, Formatting.Indented));

            //IAutonomousApp client = AutonomousApp.Initialize()
            //                                     .WithDefaults()
            //                                     .Authenticate()
            //                                     .GetClient();

            //Assert.IsNotNull(client);
        }
    }
}