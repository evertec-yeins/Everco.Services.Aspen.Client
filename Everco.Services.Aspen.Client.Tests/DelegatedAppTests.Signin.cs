// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.Signin.cs" company="Evertec Colombia">
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
    using Everco.Services.Aspen.Client.Auth;
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
        /// Se emite un token de autenticación para un usuario cuando la credencial corresponde a una válida por el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UserSigninRequestWorks()
        {
            IDelegatedApp client = AuthenticateNoCache();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si la credencial del usuario no es válida.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void InvalidUserCredentialThrows()
        {
            string password = Guid.Empty.ToString();
            UserIdentity invalidCredentialIdentity = new UserIdentity(
                UserIdentity.Default.DocType,
                UserIdentity.Default.DocNumber,
                password);

            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(invalidCredentialIdentity)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("97414"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Combinación de usuario y contraseña invalida. Por favor revise los valores ingresados e intente de nuevo", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si el usuario no es reconocido en el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UnrecognizedUserThrows()
        {
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string password = Guid.Empty.ToString();
            UserIdentity unrecognizedUserIdentity = new UserIdentity(fixedDocType, randomDocNumber, password);

            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(unrecognizedUserIdentity)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("97412"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Combinación de usuario y contraseña invalida. Por favor revise los valores ingresados e intente de nuevo", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando el usario está bloqueado.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UserLockoutThrows()
        {
            UserIdentity userIdentity = UserIdentity.Default;
            SqlDataContext.Default.EnsureUserIsLocked(userIdentity.DocType, userIdentity.DocNumber);
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(userIdentity)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("97413"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Usuario está bloqueado por superar el número máximo de intentos de sesión inválidos", exception.Message);
            SqlDataContext.Default.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando el usuario existe en el sistema pero no tiene credenciales establecidas.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void MissingUserCredentialProfilePropertiesThrows()
        {
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string password = Guid.Empty.ToString();
            UserIdentity tempUserIdentity = new UserIdentity(fixedDocType, randomDocNumber, password);
            SqlDataContext.Default.AddUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(tempUserIdentity)
                        .GetClient();
                });
            
            SqlDataContext.Default.RemoveUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
            Assert.That(exception.EventId, Is.EqualTo("97416"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Combinación de usuario y contraseña invalida. Por favor revise los valores ingresados e intente de nuevo", exception.Message);
        }

        /// <summary>
        /// Se produce un error interno de servidor cuando el formato de la contraseña en el perfil del usuario no es reconocido por el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void InvalidSecretFormatUserProfilePropertiesThrows()
        {
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            string password = Guid.Empty.ToString();
            IAppIdentity appIdentity = DelegatedAppIdentity.Default;
            IUserIdentity tempUserIdentity = new UserIdentity(fixedDocType, randomDocNumber, password);
            Dictionary<string, string> userProfile = new Dictionary<string, string>()
                                                         {
                                                             { "Secret", password },
                                                             { "SecretFormat", "InvalidTypeName" }
                                                         };
            SqlDataContext.Default.AddUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber, appIdentity.ApiKey, userProfile);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(appIdentity)
                        .AuthenticateNoCache(tempUserIdentity)
                        .GetClient();
                });
            
            SqlDataContext.Default.RemoveUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
            Assert.That(exception.EventId, Is.EqualTo("97417"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            StringAssert.IsMatch("No es posible verificar las credenciales del usuario.", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación por bloqueo de intentos inválidos.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UserLockedOutForFailedAttemptsSignin()
        {
            string password = Guid.Empty.ToString();
            IAppIdentity appIdentity = DelegatedAppIdentity.Default;
            UserIdentity userIdentity = new UserIdentity(
                UserIdentity.Default.DocType,
                UserIdentity.Default.DocNumber,
                password);

            SqlDataContext.Default.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
            int maxFailedPasswordAttempt = SqlDataContext.Default.GetAppMaxFailedPasswordAttempt(appIdentity.ApiKey);
            
            void Authenticate() =>
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(appIdentity)
                    .AuthenticateNoCache(userIdentity)
                    .GetClient();

            AspenException exception;
            for (int index = 1; index < maxFailedPasswordAttempt; index++)
            {
                exception = Assert.Throws<AspenException>(Authenticate);
                Assert.That(exception.EventId, Is.EqualTo("97414"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                StringAssert.IsMatch("Combinación de usuario y contraseña invalida. Por favor revise los valores ingresados e intente de nuevo", exception.Message);
            }

            exception = Assert.Throws<AspenException>(Authenticate);
            Assert.That(exception.EventId, Is.EqualTo("97415"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Usuario ha sido bloqueado por superar el número máximo de intentos de sesión inválidos", exception.Message);
            SqlDataContext.Default.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando la aplicación no coincide con el alcance esperado.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void ApiKeyScopeMismatchThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache(UserIdentity.Default)
                        .GetClient();
                });
            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operación. Alcance requerido: 'Delegated'", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si el identificador de la aplicación no es reconocido en el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UnrecognizedApiKeyThrows()
        {
            string randomApiKey = Guid.NewGuid().ToString();
            string apiKeySecret = AutonomousAppIdentity.Default.ApiSecret;

            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(randomApiKey, apiKeySecret)
                    .AuthenticateNoCache(UserIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no válido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si el secreto de la aplicación es inválido.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void InvalidAppKeyCredentialThrows()
        {
            string recognizedApiKey = AutonomousAppIdentity.Default.ApiKey;
            string randomApiSecret = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(recognizedApiKey, randomApiSecret)
                    .AuthenticateNoCache(UserIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }
    }
}