// -----------------------------------------------------------------------
// <copyright file="AnonymousTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-09 16:40 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones anónimas del servicio.
    /// </summary>
    [TestFixture]
    public class AnonymousTests
    {
        /// <summary>
        /// Obtener la configuración de una aplicación usando un ApiKey desconocido, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppSettingsUnrecognizedApiKeyThrows()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            string unrecognizedApiKey = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetAppSettings(unrecognizedApiKey));
            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no válido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Obtener la configuración de una aplicación usando un ApiKey sin el alcance de delegado, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppSettingsUsingAutonomousApiKeyThrows()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            string autonomousApiKey = AutonomousAppIdentity.Master.ApiKey;
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetAppSettings(autonomousApiKey));
            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operación. Alcance requerido: 'Delegated'", exception.Message);
        }

        /// <summary>
        /// Obtener la configuración de una aplicación usando un ApiKey conocido y con alcance de delegado funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppSettingsWorks()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            string recognizedApiKey = DelegatedAppIdentity.Master.ApiKey;
            AppMovSettings appMovSettings = client.Settings.GetAppSettings(recognizedApiKey);
            CollectionAssert.IsNotEmpty(appMovSettings);
            Assert.That(appMovSettings.GetKeyValue<bool>("enableBiometricAuth:iOS", true), Is.True);
            Assert.That(appMovSettings.GetKeyValue("enableBiometricAuth:Android", true), Is.True);
            Assert.That(appMovSettings.GetKeyValue("enableRememberUsernameLogin", true), Is.True);
        }

        /// <summary>
        /// Obtener los tipos de documento predetermiandos funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetDefaultDocTypesWorks()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            IList<DocTypeInfo> defaultDocTypes = client.Settings.GetDefaultDocTypes();
            CollectionAssert.IsNotEmpty(defaultDocTypes);
            Assert.That(defaultDocTypes.Count, Is.EqualTo(5));
            foreach (DocTypeInfo docTypeInfo in defaultDocTypes)
            {
                Assert.IsNotNull(docTypeInfo);
                Assert.That(docTypeInfo.Name, Is.Not.Null);
                Assert.That(docTypeInfo.ShortName, Is.Not.Null);
            }
        }

        /// <summary>
        /// Guardar la información del crash generado por una aplicación usando un ApiKey desconocido, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void SaveAppCrashUnrecognizedApiKeyThrows()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();

            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString() },
                { "AppStartTime", DateTime.Now.AddMinutes(-30) },
                { "AppErrorTime", DateTime.Now }
            };

            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.None);
            string unrecognizedApiKey = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() => client.Utils.SaveAppCrash(unrecognizedApiKey, Environment.UserName, errorReport));
            Assert.That(exception.EventId, Is.EqualTo("20011"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("Solicitud no autorizada", exception.Message);
        }

        /// <summary>
        /// Guardar la información del crash generado por una aplicación usando un ApiKey con alcance de autónomo, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void SaveAppCrashUsingAutonomousApiKeyThrows()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();

            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString() },
                { "AppStartTime", DateTime.Now.AddMinutes(-30) },
                { "AppErrorTime", DateTime.Now }
            };

            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.None);
            string autonomousApiKey = AutonomousAppIdentity.Master.ApiKey;
            AspenException exception = Assert.Throws<AspenException>(() => client.Utils.SaveAppCrash(autonomousApiKey, Environment.UserName, errorReport));
            Assert.That(exception.EventId, Is.EqualTo("20011"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("Solicitud no autorizada", exception.Message);
        }

        /// <summary>
        /// Guardar la información del crash (errores o bloqueos de sistema) generado por una aplicación conocida .
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void SaveAppCrashWorks()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();

            string recognizedApiKey = DelegatedAppIdentity.Master.ApiKey;
            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString() },
                { "AppStartTime", DateTime.Now.AddMinutes(-30) },
                { "AppErrorTime", DateTime.Now }
            };

            // Funciona cuando el objeto json está indentado.
            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.Indented);
            Assert.DoesNotThrow(() => client.Utils.SaveAppCrash(recognizedApiKey, Environment.UserName, errorReport));

            // Funciona también cuando el objeto json no está indentado.
            errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.None);
            Assert.DoesNotThrow(() => client.Utils.SaveAppCrash(recognizedApiKey, Environment.UserName, errorReport));
        }
    }
}