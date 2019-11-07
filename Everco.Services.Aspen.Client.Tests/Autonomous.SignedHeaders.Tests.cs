// -----------------------------------------------------------------------
// <copyright file="Autonomous.SignedHeaders.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
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
    /// Implementa las pruebas unitarias de las cabeceras de autenticación requeridas por una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public class AutonomousSignedHeadersTests
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private IAutonomousApp autonomousClient = null;

        /// <summary>
        /// Obtiene un cliente previamente autenticado.
        /// </summary>
        public IAutonomousApp AutonomousClient => this.autonomousClient ?? (this.autonomousClient = AutonomousApp.Initialize()
                                                                           .RoutingTo(EnvironmentEndpointProvider.Default)
                                                                           .WithIdentity(AutonomousAppIdentity.Default)
                                                                           .Authenticate()
                                                                           .GetClient());

        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutan antes de llamar a cada método de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Una solicitud firmada funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signed.Headers")]
        public void SignedRequestWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .Authenticate()
                    .GetClient();

            // Se usa una operación que requiere token de autenticación.
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void MissingNonceThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void NullOrEmptyNonceThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidNoncePayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void InvalidNonceFormatThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
            {
                InvalidNoncePayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                InvalidNoncePayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patrón", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se usa una operación luego de la autenticación con un nuevo nonce y debe funcionar.
            ServiceLocator.Instance.RegisterNonceGenerator(new SingleUseNonceGenerator());
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);

            // Se una nuevamente el mismo nonce y debe fallar ya que se está reutilizando.
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicación", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void MissingEpochThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void NullOrEmptyEpochThrows()
        {
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => null),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void InvalidEpochFormatThrows()
        {
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochExpiredThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochExceededThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochNegativeThrows()
        {
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochZeroThrows()
        {
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => this.AutonomousClient.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Token")]
        public void MissingAuthTokenThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidTokenPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Token")]
        public void NullOrEmptyAuthTokenThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidTokenPayloadClaim.WithClaimBehavior(() => null),
                InvalidTokenPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidTokenPayloadClaim.WithClaimBehavior(() => "    ")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                // Se intenta usar una operación que requiere el token de autenticación.
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Token")]
        public void InvalidAuthTokenFormatThrows()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidTokenPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }
    }
}