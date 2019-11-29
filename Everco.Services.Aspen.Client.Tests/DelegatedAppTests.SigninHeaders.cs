// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.SigninHeaders.cs" company="Evertec Colombia">
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
    using Assets;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticación requeridas por una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Autenticar un usuario cuando falta el encabezado del ApiKey no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiKey")]
        public void MissingApiKeyHeaderWhenUserSigninRequestThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(InvalidApiKeyHeader.AvoidingHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Master)
                    .AuthenticateNoCache(RecognizedUserIdentity.Master)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Cuando el encabezado del identificador de la aplicación es nulo o vacío no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiKey")]
        public void NullOrEmptyApiKeyHeaderWhenUserSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiKeyHeader.WithHeaderBehavior(() => null),
                InvalidApiKeyHeader.WithHeaderBehavior(() => string.Empty),
                InvalidApiKeyHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Master)
                        .AuthenticateNoCache(RecognizedUserIdentity.Master)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
            }
        }

        /// <summary>
        /// Cuando falta el encabezado de la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void MissingPayloadHeaderWhenUserSigninRequestThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(InvalidPayloadHeader.AvoidingHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Master)
                    .AuthenticateNoCache(RecognizedUserIdentity.Master)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        /// <summary>
        /// Cuando el valor del encabezado de la carga útil es nulo o vacío no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void NullOrEmptyPayloadHeaderWhenUserSigninRequestThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                InvalidPayloadHeader.WithHeaderBehavior(() => null),
                InvalidPayloadHeader.WithHeaderBehavior(() => string.Empty),
                InvalidPayloadHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Master)
                        .AuthenticateNoCache(RecognizedUserIdentity.Master)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación con el formato inválido (se espera un JWT) en la cabecera de la carga útil genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void InvalidFormatPayloadWhenUserSigninRequestThrows()
        {
            IHeadersManager invalidPayloadHeaderBehavior = InvalidPayloadHeader.WithHeaderBehavior(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr");
            ServiceLocator.Instance.RegisterHeadersManager(invalidPayloadHeaderBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación sin nonce en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void MissingNonceWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío.", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación usando un nonce nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void NullOrEmptyNonceWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> nonceClaimBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in nonceClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación usando un nonce con formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void InvalidFormatNonceWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> nonceClaimBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in nonceClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación usando un nonce que ya fue procesado no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterNonceGenerator(new SingleUseNonceGenerator());
            IDelegatedApp client = this.GetDelegatedClient();

            // Se puede autenticar la aplicación usando el nonce la primera vez.
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            // No se podrá autentica la aplicación, cuando use el mismo nonce por segunda vez.
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicación", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando falta el epoch en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void MissingEpochWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando el epoch es nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void NullOrEmptyEpochWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> epochClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => null),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in epochClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece un epoch con formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void InvalidFormatEpochWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> epochClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
            };

            foreach (IPayloadClaimsManager behavior in epochClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación con un epoch que ya expiró en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochExpiredWhenUserSigninRequestThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación con un epoch muy adelante en el futuro en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochExceededWhenUserSigninRequestThrows()
        {
            int randomDays = new Random().Next(5, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación con un epoch negativo en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochNegativeWhenUserSigninRequestThrows()
        {
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un número.", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación con un epoch igual a cero en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochZeroWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch está fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación sin el tipo de documento del usuario en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocType")]
        public void MissingDocTypeWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDocTypePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece el tipo de documento del usuario como nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocType")]
        public void NullOrEmptyDocTypeWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> docTypeClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => null),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in docTypeClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación usando tipos documentos inválidos en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocType")]
        public void UnsupportedDocTypeWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> docTypeClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "01"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "10"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "xx"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "XX"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "Xx"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "xX"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "cc"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "Cc")
            };

            foreach (IPayloadClaimsManager behavior in docTypeClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no se reconoce como un tipo de identificación", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación sin el número de documento del usuario en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocNumber")]
        public void MissingDocNumberWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDocNumberPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece el número de documento del usuario como nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocNumber")]
        public void NullOrEmptyDocNumberWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> docNumberClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => null),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in docNumberClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece el número de documento del usuario con un formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DocNumber")]
        public void InvalidFormatDocNumberWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> docNumberClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "a"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "abcdef"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "A0b1C2d3e4f5g6H7"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "$123456@123#456*"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "97165682970991858533737047")
            };

            foreach (IPayloadClaimsManager behavior in docNumberClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch(@"'DocNumber' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación cuando sin el identificador de dispositivo en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DeviceId")]
        public void MissingDeviceIdWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDeviceIdPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece un identificador de dispositivo nulo o vacío en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DeviceId")]
        public void NullOrEmptyDeviceIdWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> deviceIdClaimBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in deviceIdClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se establece un identificador de dispositivo con un formato inválido en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DeviceId")]
        public void InvalidFormatDeviceIdWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> deviceIdClaimBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in deviceIdClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DeviceId' debe coincidir con el patrón", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticación cuando se usa un identificador de dispositivo aleatorio en la carga útil funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.DeviceId")]
        public void UsingRandomDeviceIdWhenUserSigninRequestWorks()
        {
            IPayloadClaimsManager randomDeviceIdClaimBehavior = InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => $"MyRandomDevice-{new Random().Next(999999, 9999999)}");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(randomDeviceIdClaimBehavior);
            IDelegatedApp client = this.GetDelegatedClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando falta la contraseña del usuario en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Password")]
        public void MissingPasswordWhenUserSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidPasswordPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Password' no puede ser nulo ni vacío", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticación cuando falta la contraseña del usuario en la carga útil no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Password")]
        public void NullOrEmptyPasswordWhenUserSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> passwordClaimBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidPasswordPayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidPasswordPayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidPasswordPayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in passwordClaimBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Password' no puede ser nulo ni vacío", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicación sin la cabecera de la versión del API solicitada funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void MissingApiVersionHeaderWhenUserSigninRequestWorks()
        {
            ServiceLocator.Instance.RegisterHeadersManager(InvalidApiVersionHeader.AvoidingHeader());
            IDelegatedApp client = this.GetDelegatedClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Autenticar una aplicación con valores nulos o vacíos en la cabecera de la versión del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void NullEmptyApiVersionHeaderWhenUserSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => null),
                InvalidApiVersionHeader.WithHeaderBehavior(() => string.Empty),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato válido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicación con valores inválidos en la cabecera de la versión del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void InvalidApiVersionHeaderFormatWhenUserSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "abc"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => Guid.NewGuid().ToString()),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "123"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1,0"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1A"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "-1.0")
            };

            foreach (IHeadersManager headerBehavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato válido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicación con valores no soportados en la cabecera de la versión del API no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void UnsupportedApiVersionHeaderWhenUserSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "0.1"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999.999999")
            };

            foreach (IHeadersManager headerBehavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
                Assert.That(exception.EventId, Is.EqualTo("99005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un valor admitido para el encabezado personalizado 'X-PRO-Api-Version'", exception.Message);
            }
        }
    }
}