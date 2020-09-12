// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.SignedHeaders.cs" company="Evertec Colombia">
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
    using Assets;
    using Entities;
    using Fluent;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticación requeridas por una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando falta la cabecera del ApiKey genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed.Headers.ApiKey")]
        public void MissingApiKeyHeaderWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
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
        public void NullOrEmptyApiKeyHeaderWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
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
            IAutonomousApp client = this.GetAutonomousClient();
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
            IAutonomousApp client = this.GetAutonomousClient();
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
            IAutonomousApp client = this.GetAutonomousClient();
            IHeadersManager invalidPayloadHeaderBehavior = InvalidPayloadHeader.WithHeaderBehavior(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr");
            ServiceLocator.Instance.RegisterHeadersManager(invalidPayloadHeaderBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando falta el nonce en la carga útil de la solicitud no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void MissingNonceSignedRequestWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un nonce como nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NullOrEmptyNonceWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
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

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un nonce con formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void InvalidFormatNonceWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
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
        /// Una solicitud a una operación que requiere firma cuando se establece un nonce que ya fue utilizado en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedWhenSignedRequestThrows()
        {
            // Autenticación de la aplicación.
            IAutonomousApp client = this.GetAutonomousClient();

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

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando falta el epoch en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void MissingEpochWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch como nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void NullOrEmptyEpochWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
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
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch con formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void InvalidFormatEpochWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
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
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch que ya expiró en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochExpiredWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch muy adelante en el futuro en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochExceededWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch negativo en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochNegativeWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un epoch igual a cero en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Epoch")]
        public void EpochZeroWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando falta el token de autenticación en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void MissingTokenWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidTokenPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando el token de autenticación es nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void NullOrEmptyTokenWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<IPayloadClaimsManager> tokenClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidTokenPayloadClaim.WithClaimBehavior(() => null),
                InvalidTokenPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidTokenPayloadClaim.WithClaimBehavior(() => "    ")
            };

            foreach (IPayloadClaimsManager behavior in tokenClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Token' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se establece un token de autenticación con formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signed.Headers.Payload.Token")]
        public void InvalidFormatTokenWhenSignedRequestThrows()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IPayloadClaimsManager invalidTokenClaimBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(invalidTokenClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }
    }
}