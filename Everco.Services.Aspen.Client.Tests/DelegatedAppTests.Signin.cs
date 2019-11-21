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
            IDelegatedApp client = this.GetDelegatedClient();
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
                UserIdentity.Master.DocType,
                UserIdentity.Master.DocNumber,
                password);

            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Master)
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
                    .WithIdentity(DelegatedAppIdentity.Master)
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
            IUserIdentity userIdentity = UserIdentity.Master;
            SqlDataContext.EnsureUserIsLocked(userIdentity.DocType, userIdentity.DocNumber);
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Master)
                    .AuthenticateNoCache(userIdentity)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("97413"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("Usuario está bloqueado por superar el número máximo de intentos de sesión inválidos", exception.Message);
            SqlDataContext.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
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
            SqlDataContext.EnsureUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Master)
                        .AuthenticateNoCache(tempUserIdentity)
                        .GetClient();
                });
            
            SqlDataContext.RemoveUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            IUserIdentity tempUserIdentity = new UserIdentity(fixedDocType, randomDocNumber, password);
            Dictionary<string, string> userProfile = new Dictionary<string, string>()
                                                         {
                                                             { "Secret", password },
                                                             { "SecretFormat", "InvalidTypeName" }
                                                         };
            SqlDataContext.EnsureUserAndProfileInfo(
                tempUserIdentity.DocType,
                tempUserIdentity.DocNumber,
                appIdentity.ApiKey,
                userProfile);

            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(appIdentity)
                        .AuthenticateNoCache(tempUserIdentity)
                        .GetClient();
                });
            
            SqlDataContext.RemoveUserInfo(tempUserIdentity.DocType, tempUserIdentity.DocNumber);
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
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            UserIdentity userIdentity = new UserIdentity(
                UserIdentity.Master.DocType,
                UserIdentity.Master.DocNumber,
                password);

            SqlDataContext.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
            int maxFailedPasswordAttempt = SqlDataContext.GetAppMaxFailedPasswordAttempt(appIdentity.ApiKey);
            
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
            SqlDataContext.EnsureUserIsNotLocked(userIdentity.DocType, userIdentity.DocNumber);
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
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .AuthenticateNoCache(UserIdentity.Master)
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
            string apiKeySecret = AutonomousAppIdentity.Master.ApiSecret;

            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(randomApiKey, apiKeySecret)
                    .AuthenticateNoCache(UserIdentity.Master)
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
            string recognizedApiKey = AutonomousAppIdentity.Master.ApiKey;
            string randomApiSecret = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(recognizedApiKey, randomApiSecret)
                    .AuthenticateNoCache(UserIdentity.Master)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando la aplicación esta inhabilitada.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void ApiKeyDisabledUserSigninRequestThrows()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.DisableApp(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("20006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey está desactivado. Póngase en contacto con el administrador", exception.Message);
            SqlDataContext.EnableApp(appIdentity.ApiKey);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando la aplicación requiere cambiar su secreto.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void AppRequiresChangeSecretWhenUserSigninRequestThrows()
        {
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.EnsureAppRequiresChangeSecret(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => GetDelegatedClient());
            Assert.That(exception.EventId, Is.EqualTo("20009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.UpgradeRequired));
            StringAssert.IsMatch("Necesita actualizar el secreto de la aplicación.", exception.Message);
            SqlDataContext.EnsureAppNotRequiresChangeSecret(appIdentity.ApiKey);
        }
    }
}