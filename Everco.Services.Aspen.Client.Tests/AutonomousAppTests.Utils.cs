// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Utils.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones para utilidades varias soportadas para una aplicación con alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
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
    }
}