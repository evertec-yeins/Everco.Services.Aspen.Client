// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Signin.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Net;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticación requeridas por una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Autenticar una aplicación autónoma funciona.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void AppSigninRequestWorks()
        {
            IAutonomousApp client = AuthenticateNoCache();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si la aplicación no es reconocida en el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UnrecognizedApiKeyThrows()
        {
            string randomApiKey = Guid.NewGuid().ToString();
            string apiKeySecret = AutonomousAppIdentity.Default.ApiSecret;
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(randomApiKey, apiKeySecret)
                        .AuthenticateNoCache()
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no válido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación si la credencial de la aplicación no es válida.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void InvalidAppCredentialThrows()
        {
            string recognizedApiKey = AutonomousAppIdentity.Default.ApiKey;
            string invalidApiSecret = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(recognizedApiKey, invalidApiSecret)
                        .AuthenticateNoCache()
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicación cuando se usa un ApiKey con alcance de delegada no funciona.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void ApiKeyScopeMismatchThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });
            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operación. Alcance requerido: 'Autonomous'", exception.Message);
        }
    }
}