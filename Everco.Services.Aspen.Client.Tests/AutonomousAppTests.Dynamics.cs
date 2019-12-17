// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Dynamics.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-02 03:50 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Fluent;
    using Identities;
    using NUnit.Framework;

    [TestFixture]
    public partial class AutonomousAppTests
    {
        [Test]
        [Category("Modules.Dynamics")]
        public void XYZ()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                                                 .RoutingTo(TestingEndpointProvider.Default)
                                                 .WithIdentity(AutonomousAppIdentity.Master)
                                                 .AuthenticateNoCache()
                                                 .GetClient();

            AspenException exception = Assert.Throws<AspenException>(() => client.Dynamics.Post("diagnostics/dump/998fdd73"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.EventId, Is.EqualTo("15840"));
        }

        [Test]
        [Category("Modules.Dynamics")]
        public void RecognizedEndpoint()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                                                 .RoutingTo(TestingEndpointProvider.Default)
                                                 .WithIdentity(AutonomousAppIdentity.Master)
                                                 .AuthenticateNoCache()
                                                 .GetClient();

            AspenException exception = Assert.Throws<AspenException>(() => client.Dynamics.Post("atorres/calc", new KeyValuePair<string, object>("Input", "20")));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("Modules.Dynamics")]
        public void RecognizedEndpointWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                                                 .RoutingTo(TestingEndpointProvider.Default)
                                                 .WithIdentity(AutonomousAppIdentity.Master)
                                                 .AuthenticateNoCache()
                                                 .GetClient();

            int a = new Random(Guid.NewGuid().GetHashCode()).Next(0, int.MaxValue);
            int b = new Random(Guid.NewGuid().GetHashCode()).Next(0, int.MaxValue);
            var request = new CustomRequest(a, b, '+');
            var response = client.Dynamics.Post<CustomRequest, CustomResponse>("atorres/calc", request);
            Assert.That(response.Result, Is.EqualTo(a+b));
            Assert.That(response.ResponseCode, Is.EqualTo(200));
            Assert.That(response.ResponseCode, Is.EqualTo("OK"));
        }
    }

    public class CustomResponse
    {
        public int Result { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class CustomRequest
    {
        public CustomRequest(int a, int b, char @operator)
        {
            this.Input = $"{a}{@operator}{b}";
        }

        public string Input { get; set; }
    }
}
