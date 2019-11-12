// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones para utilidades varias soportadas para una aplicación con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtener los tipos de documento predetermiandos funciona.
        /// </summary>
        [Test]
        [Category("Modules.Utils")]
        public void GetDefaultDocTypesWorks()
        {
            IList<DocTypeInfo> defaultDocTypes = Client.Utils.GetDefaultDocTypes();
            CollectionAssert.IsNotEmpty(defaultDocTypes);
        }

        /// <summary>
        /// Cifra una cadena de texto usando el algoritmo de cifrado del servicio funciona.
        /// </summary>
        [Test]
        [Category("Modules.Utils")]
        public void EncryptValueWorks()
        {
            string encryptedValue = Client.Utils.Encrypt("colombia");
            Assert.IsNotEmpty(encryptedValue);
        }

        /// <summary>
        /// Guardar la información del crash (errores o bloqueos de sistema) generado por una aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Utils")]
        public void SaveAppCrashWorks()
        {
            Dictionary<string, object> errorReportInfo = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString() },
                { "AppStartTime", DateTime.Now },
                { "AppErrorTime", DateTime.Now }
            };

            string errorReport = JsonConvert.SerializeObject(errorReportInfo, Formatting.Indented);
            Assert.DoesNotThrow(() => Client.Utils.SaveAppCrash(errorReport, Environment.UserName));
        }
    }
}