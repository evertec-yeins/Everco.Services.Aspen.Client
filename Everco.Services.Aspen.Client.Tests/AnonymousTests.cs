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
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Entities;
    using Fluent;
    using Identities;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones anónimas del servicio.
    /// </summary>
    [TestFixture]
    public class AnonymousTests
    {
        /// <summary>
        /// Obtener la configuración de una aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppSettingsFromCacheWorks()
        {
            IAnonymous client = this.GetAnonymousClient(CachePolicy.CacheIfAvailable);
            string recognizedApiKey = DelegatedAppIdentity.Master.ApiKey;
            Assert.DoesNotThrow(() => client.Settings.GetAppSettings(recognizedApiKey));
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                AppMovSettings appMovSettings = client.Settings.GetAppSettings(recognizedApiKey);
                watch.Stop();
                CollectionAssert.IsNotEmpty(appMovSettings);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtener la configuración de una aplicación usando un ApiKey desconocido, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppSettingsUnrecognizedApiKeyThrows()
        {
            IAnonymous client = this.GetAnonymousClient();
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
            IAnonymous client = this.GetAnonymousClient()
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
            IAnonymous client = this.GetAnonymousClient()
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
            IAnonymous client = this.GetAnonymousClient()
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
        /// Obtener todos los recursos de la parametrización asíncronamente funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetSettingsAsynchronouslyWorks()
        {
            IAnonymous client = this.GetAnonymousClient();

            IList<DocTypeInfo> docTypes = Enumerable.Empty<DocTypeInfo>().ToList();
            string recognizedApiKey = DelegatedAppIdentity.Master.ApiKey;
            AppMovSettings appMovSettings = new AppMovSettings();

            Assert.DoesNotThrowAsync(async () => docTypes = await client.Settings.GetDefaultDocTypesAsync());
            Assert.DoesNotThrowAsync(async () => appMovSettings = await client.Settings.GetAppSettingsAsync(recognizedApiKey));
            CollectionAssert.IsNotEmpty(docTypes);
            CollectionAssert.IsNotEmpty(appMovSettings);
        }

        /// <summary>
        /// Guardar la información del crash generado por una aplicación usando un ApiKey desconocido, no funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void SaveAppCrashUnrecognizedApiKeyThrows()
        {
            IAnonymous client = this.GetAnonymousClient()
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
            IAnonymous client = this.GetAnonymousClient()
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
            IAnonymous client = this.GetAnonymousClient()
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

        /// <summary>
        /// Obtiene un cliente para a partir de la aplicación autónoma de pruebas, omitiendo los valores almacenados en memoria.
        /// </summary>
        /// <param name="cachePolicy">La política para el tratamiento de la información almacenada por caché.</param>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        private IAnonymous GetAnonymousClient(CachePolicy cachePolicy = CachePolicy.BypassCache) =>
            Anonymous.Initialize(cachePolicy)
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
    }
}