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
    using Identity;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras personalizadas requeridas para firmar una solicitud por una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando falta la cabecera del ApiKey genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed.Headers.ApiKey")]
        public void MissingApiKeyHeaderWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterHeadersManager(InvalidApiKeyHeader.AvoidingHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando la cabecera del ApiKey es nula o vacía genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed.Headers.ApiKey")]
        public void NullOrEmptyApiKeyHeaderWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
                                                         {
                                                             InvalidApiKeyHeader.WithHeaderBehavior(() => null),
                                                             InvalidApiKeyHeader.WithHeaderBehavior(() => string.Empty),
                                                             InvalidApiKeyHeader.WithHeaderBehavior(() => "      ")
                                                         };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación sin el encabezado de la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload")]
        public void MissingPayloadHeaderWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterHeadersManager(InvalidPayloadHeader.AvoidingHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación con el encabezado de la carga útil nula o vacía no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload")]
        public void NullOrEmptyPayloadHeaderWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IHeadersManager> payloadHeaderBehaviors = new List<IHeadersManager>()
            {
                InvalidPayloadHeader.WithHeaderBehavior(() => null),
                InvalidPayloadHeader.WithHeaderBehavior(() => string.Empty),
                InvalidPayloadHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación con el formato inválido (se espera un JWT) en la cabecera de la carga útil genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload")]
        public void InvalidFormatPayloadWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IHeadersManager invalidPayloadHeaderBehavior = InvalidPayloadHeader.WithHeaderBehavior(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr");
            ServiceLocator.Instance.RegisterHeadersManager(invalidPayloadHeaderBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el nonce en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void MissingNonceWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío.", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el nonce es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NullOrEmptyNonceWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "    ")
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

        /// <summary>
        /// Una operación que requiere firma cuando se establece un nonce con formato inválido en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void InvalidFormatNonceWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
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

        /// <summary>
        /// Firmar una solicitud usando un nonce ya utilizado no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedWhenSignedRequestThrows()
        {
            // Se usa un nonce aleatorio con la autenticación del cliente...
            ServiceLocator.Instance.RegisterNonceGenerator(new SingleUseNonceGenerator());
            IDelegatedApp client = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .Authenticate(RecognizedUserIdentity.Master)
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
        public void MissingTokenWhenSignedRequestThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .Authenticate(RecognizedUserIdentity.Master)
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
        public void NullOrEmptyTokenWhenSignedRequestThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Master)
                .Authenticate(RecognizedUserIdentity.Master)
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
        public void InvalidFormatTokenWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IPayloadClaimsManager invalidFormatBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(invalidFormatBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el epoch en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void MissingEpochWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el epoch es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void NullOrEmptyEpochWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => null),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
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
        public void InvalidFormatEpochWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
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
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
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
        public void EpochExpiredWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch muy adelante en el futuro no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochExceededWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch negativo no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochNegativeWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
        }

        /// <summary>
        /// Una solicitud con un epoch igual a cero no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochZeroWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando falta el identificador del dispositivo en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.DeviceId")]
        public void MissingDeviceIdWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDeviceIdPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el identificador del dispositivo es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.DeviceId")]
        public void NullOrEmptyDeviceIdWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
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
        public void InvalidFormatDeviceIdWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
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
        public void MissingUsernameWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidUsernamePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Username' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una operación que requiere firma cuando el username es nulo o vacío en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Username")]
        public void NullOrEmptyUsernameWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Username' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una operación que requiere firma cuando el formato del username en la carga útil de la solicitud no es el esperado.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Username")]
        public void InvalidFormatUsernameWhenSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "CC|123456"),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "cc-abcdef"),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "1234656"),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => "noreply@myemail.com"),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => Environment.UserName),
                                                                    InvalidUsernamePayloadClaim.WithClaimBehavior(() => Guid.NewGuid().ToString())
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Username' debe coincidir con el patrón", exception.Message);
            }
        }
    }
}