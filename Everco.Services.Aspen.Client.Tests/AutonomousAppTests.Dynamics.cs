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
    using NUnit.Framework;

    [TestFixture]
    public partial class AutonomousAppTests
    {
        [Test]
        [Category("Modules.Dynamics")]
        public void DumpTraceNotFoundWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Dynamics.Post("diagnostics/dump/998fdd73"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.EventId, Is.EqualTo("15840"));
        }

        [Test]
        [Category("Modules.Dynamics")]
        public void RecognizedEndpoint()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            AspenException exception = Assert.Throws<AspenException>(() => client.Dynamics.Post("client/test/calc", new KeyValuePair<string, object>("Input", "20")));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("Modules.Dynamics")]
        public void RecognizedEndpointWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            int a = new Random(Guid.NewGuid().GetHashCode()).Next(10, 100);
            int b = new Random(Guid.NewGuid().GetHashCode()).Next(10, 100);
            CustomRequest request = new CustomRequest(a, b, '+');
            CustomResponse response = client.Dynamics.Post<CustomRequest, CustomResponse>("client/test/calc", request);
            Assert.That(response.Result, Is.EqualTo(a+b));
            Assert.That(response.Text, Is.Null.Or.Empty);
        }
    }

    public class CustomResponse
    {
        public CustomResponse()
        {
        }

        public string Text { get; set; }

        public int Result { get; set; }
    }

    public class CustomRequest
    {
        public CustomRequest()
        {
        }

        public CustomRequest(int a, int b, char @operator)
        {
            this.Input = $"{a}{@operator}{b}";
        }

        public string Input { get; set; }
    }
}
