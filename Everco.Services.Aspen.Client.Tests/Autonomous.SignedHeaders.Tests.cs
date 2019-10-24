// -----------------------------------------------------------------------
// <copyright file="Autonomous.SignedHeaders.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class AutonomousSignedHeadersTests
    {
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
            ServiceLocator.Instance.RegisterWebProxy(new WebProxy("http://192.168.2.70:8080", true));
            ServiceLocator.Instance.RegisterLoggingProvider(new ConsoleLoggingProvider());
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void SignedRequestWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .Authenticate()
                    .GetClient();

            // Se usa una operación que requiere token de autenticación.
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void MissingNonceThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            ServiceLocator.Instance.RegisterHeadersManager(new MissingNoncePayloadHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void NullOrEmptyNonceThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNoncePayloadHeader(HeaderValueBehavior.Null),
                new MissingNoncePayloadHeader(HeaderValueBehavior.Empty),
                new MissingNoncePayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void InvalidNonceFormatThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNoncePayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingNoncePayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void NonceAlreadyProcessedThrows()
        {
            Guid duplicatedNonce = Guid.NewGuid();
            ServiceLocator.Instance.RegisterNonceGenerator(new DuplicatedNonceGenerator(duplicatedNonce));
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se usa una operación luego de la autenticación con el mismo nonce y debe fallar ya que se está reutilizando.
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicación", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void MissingEpochThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            ServiceLocator.Instance.RegisterHeadersManager(new MissingEpochPayloadHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void NullOrEmptyEpochThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochPayloadHeader(HeaderValueBehavior.Null),
                new MissingEpochPayloadHeader(HeaderValueBehavior.Empty),
                new MissingEpochPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void InvalidEpochFormatThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochPayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingEpochPayloadHeader(HeaderValueBehavior.MinLengthRequired),
                new MissingEpochPayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void EpochExpiredThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            int randomDays = new Random().Next(5, 10);
            IList<IEpochGenerator> epochBehaviors = new List<IEpochGenerator>()
            {
                new DatePickerEpochGenerator(-randomDays),
                new DatePickerEpochGenerator(randomDays),
            };

            foreach (IEpochGenerator behavior in epochBehaviors)
            {
                ServiceLocator.Instance.RegisterEpochGenerator(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15851"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
                StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void MissingAuthTokenThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterHeadersManager(new MissingAuthTokenClaimOnPayloadHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void NullOrEmptyAuthTokenThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.Null),
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.Empty),
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                // Se intenta usar una operación que requiere el token de autenticación.
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void InvalidAuthTokenFormatThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterHeadersManager(new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.UnexpectedFormat));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }
    }
}