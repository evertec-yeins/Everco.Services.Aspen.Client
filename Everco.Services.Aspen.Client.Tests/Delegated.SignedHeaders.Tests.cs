// -----------------------------------------------------------------------
// <copyright file="Delegated.SignedHeaders.Tests.cs" company="Evertec Colombia">
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
    public class DelegatedSignedHeadersTests
    {
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
            ServiceLocator.Instance.RegisterWebProxy(new WebProxy("http://192.168.2.70:8080", true));
            ServiceLocator.Instance.RegisterLoggingProvider(new ConsoleLoggingProvider());
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void SignedRequestWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();

            // Se usa una operación que requiere token de autenticación.
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void NonceAlreadyProcessedThrows()
        {
            Guid duplicatedNonce = Guid.NewGuid();
            ServiceLocator.Instance.RegisterNonceGenerator(new DuplicatedNonceGenerator(duplicatedNonce));
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se usa una operación luego de la autenticación con el mismo nonce y debe fallar ya que se está reutilizando.
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicación", exception.Message);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void MissingAuthTokenThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterHeadersManager(new MissingAuthTokenClaimOnPayloadHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void NullOrEmptyAuthTokenThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
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
        [Category("Delegated.Signed.Headers")]
        public void InvalidAuthTokenFormatThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
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