// -----------------------------------------------------------------------
// <copyright file="AnonymousTests.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-09 16:40 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Providers;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Everco.Services.Aspen.Entities;
    using Identity;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones anónimas del servicio.
    /// </summary>
    [TestFixture]
    public partial class AnonymousTests
    {
        /// <summary>
        /// Obtener los tipos de documento predetermiandos funciona.
        /// </summary>
        [Test]
        [Category("Anonymous.Utils")]
        public void GetDefaultDocTypesWorks()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            IList<DocTypeInfo> defaultDocTypes = client.Utils.GetDefaultDocTypes();
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
        /// Guardar la información del crash (errores o bloqueos de sistema) generado por una aplicación conocida .
        /// </summary>
        [Test]
        [Category("Anonymous.Utils")]
        public void SaveAppCrashWorks()
        {
            IAnonymous client = Anonymous.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .GetClient();
            string recognizedApiKey = DelegatedAppIdentity.Master.ApiKey;
            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
                                                             {
                                                                 { "Id", Guid.NewGuid().ToString() },
                                                                 { "AppStartTime", DateTime.Now },
                                                                 { "AppErrorTime", DateTime.Now }
                                                             };

            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.Indented);
            Assert.DoesNotThrow(() => client.Utils.SaveAppCrash(recognizedApiKey, Environment.UserName, errorReport));

            errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.None);
            Assert.DoesNotThrow(() => client.Utils.SaveAppCrash(recognizedApiKey, Environment.UserName, errorReport));
        }
    }
}
