// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.SignedHeaders.cs" company="Evertec Colombia">
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
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras personalizadas requeridas para firmar una solicitud por una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Una operación que requiere firma cuando falta el nonce en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void MissingNonceSignedRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío.", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el nonce es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NullOrEmptyNonceSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "    ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando se establece un nonce con formato inválido en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void InvalidFormatNonceSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Firmar una solicitud usando un nonce ya utilizado no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedSignedRequestThrows()
        {
            // Se usa un nonce aleatorio con la autenticación del cliente...
            ServiceLocator.Instance.RegisterNonceGenerator(new SingleUseNonceGenerator());
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            // Se usa una operación luego de la autenticación con el mismo nonce y debe fallar ya que se está reutilizando...
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicación", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el token de autenticación en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void MissingTokenSignedRequestThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            // Se intenta usar una operación que requiere el token de autenticación.
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidTokenPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el token de autenticación es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void NullOrEmptyTokenSignedRequestThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidTokenPayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidTokenPayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidTokenPayloadClaim.WithClaimBehavior(() => "    ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando se establece un token de autenticación con formato inválido en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void InvalidFormatTokenSignedRequestThrows()
        {
            IPayloadClaimsManager invalidFormatBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(invalidFormatBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el epoch en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void MissingEpochSignedRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el epoch es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void NullOrEmptyEpochSignedRequestThrows()
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
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando se establece un epoch con formato inválido en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void InvalidFormatEpochSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "abcdef"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "a123b"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "A123B"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => Guid.NewGuid().ToString())
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud con un epoch que ya expiró no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochExpiredSignedRequestThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch muy adelante en el futuro no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochExceededSignedRequestThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch negativo no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochNegativeSignedRequestThrows()
        {
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch igual a cero no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochZeroSignedRequestThrows()
        {
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el identificador del dispositivo en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.DeviceId")]
        public void MissingDeviceIdSignedRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDeviceIdPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el identificador del dispositivo es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.DeviceId")]
        public void NullOrEmptyDeviceIdSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando se establece un identificador de dispositivo con formato inválido en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.DeviceId")]
        public void InvalidFormatDeviceIdSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DeviceId' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el username en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Username")]
        public void MissingUsernameSignedRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidUsernamePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Username' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el username es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Username")]
        public void NullOrEmptyUsernameSignedRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => Client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Username' no puede ser nulo ni vacío", exception.Message);
            }
        }
    }
}