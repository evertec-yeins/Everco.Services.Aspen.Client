// -----------------------------------------------------------------------
// <copyright file="Anonymous.Utils.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class AnonymousUtilsTests
    {
        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán antes de llamar a cada método de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        [Test]
        [Category("Anonymous.Utils")]
        public void GetDefaultDocTypesWorks()
        {
            IAutonomousApp autonomousClient = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .AuthenticateNoCache()
                .GetClient();

            CollectionAssert.IsNotEmpty(autonomousClient.Utils.GetDefaultDocTypes());

            IDelegatedApp delegatedClient = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            CollectionAssert.IsNotEmpty(delegatedClient.Utils.GetDefaultDocTypes());
        }

        [Test]
        [Category("Anonymous.Utils")]
        public void EncryptValueWorks()
        {
            IAutonomousApp autonomousClient = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .AuthenticateNoCache()
                .GetClient();

            Assert.IsNotEmpty(autonomousClient.Utils.Encrypt("colombia"));

            IDelegatedApp delegatedClient = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            Assert.IsNotEmpty(delegatedClient.Utils.Encrypt("colombia"));
        }

        [Test]
        [Category("Anonymous.Utils")]
        public void SaveAppCrashWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString() },
                { "AppStartTime", DateTime.Now },
                { "AppErrorTime", DateTime.Now }
            };
            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.Indented);
            Assert.DoesNotThrow(() => client.Utils.SaveAppCrash(errorReport, Environment.UserName));
        }
    }
}