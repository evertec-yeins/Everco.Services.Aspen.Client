// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Settings.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Entities;
    using Fluent;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las entidades de información relacionadas con parametrización del sistema para una aplicación de alcance de autónoma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Obtener la lista de operadores de telefonía móvil soportados para la aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetCarriersFromCacheWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient(CachePolicy.CacheIfAvailable);
            Assert.DoesNotThrow(() => client.Settings.GetCarriers());
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                IList<CarrierInfo> carriers = client.Settings.GetCarriers();
                watch.Stop();
                CollectionAssert.IsNotEmpty(carriers);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtener la lista de operadores de telefonía móvil soportados para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetCarriersWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<CarrierInfo> carriers = client.Settings.GetCarriers();
            CollectionAssert.IsNotEmpty(carriers);
        }

        /// <summary>
        /// Obtener los tipos de documento de la aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetDocTypesFromCacheWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient(CachePolicy.CacheIfAvailable);
            Assert.DoesNotThrow(() => client.Settings.GetDocTypes());
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
                watch.Stop();
                CollectionAssert.IsNotEmpty(docTypes);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtener los tipos de documento de la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetDocTypesWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        /// <summary>
        /// Obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetPaymentTypesFromCacheWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient(CachePolicy.CacheIfAvailable);
            Assert.DoesNotThrow(() => client.Settings.GetPaymentTypes());
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                IList<PaymentTypeInfo> paymentTypes = client.Settings.GetPaymentTypes();
                watch.Stop();
                CollectionAssert.IsNotEmpty(paymentTypes);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetPaymentTypesWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<PaymentTypeInfo> paymentTypes = client.Settings.GetPaymentTypes();
            CollectionAssert.IsNotEmpty(paymentTypes);
        }

        /// <summary>
        /// Obtiener los valores admitidos de recarga por operador para la aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTopUpValuesFromCacheWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient(CachePolicy.CacheIfAvailable);
            Assert.DoesNotThrow(() => client.Settings.GetTopUpValues());
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                IList<TopUpInfo> topUpValues = client.Settings.GetTopUpValues();
                watch.Stop();
                CollectionAssert.IsNotEmpty(topUpValues);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtiener los valores admitidos de recarga por operador para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTopUpValuesWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TopUpInfo> topUpValues = client.Settings.GetTopUpValues();
            CollectionAssert.IsNotEmpty(topUpValues);
        }

        /// <summary>
        /// Obtener la lista de los tipos de transacción soportados para la aplicación desde el caché funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTranTypesFromCacheWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient(CachePolicy.CacheIfAvailable);
            Assert.DoesNotThrow(() => client.Settings.GetTranTypes());
            for (int index = 1; index <= 10; index++)
            {
                Stopwatch watch = Stopwatch.StartNew();
                IList<TranTypeInfo> tranTypes = client.Settings.GetTranTypes();
                watch.Stop();
                CollectionAssert.IsNotEmpty(tranTypes);
                Assert.That(watch.Elapsed, Is.LessThanOrEqualTo(TimeSpan.FromMilliseconds(10)));
            }
        }

        /// <summary>
        /// Obtener la lista de los tipos de transacción soportados para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTranTypesWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            IList<TranTypeInfo> tranTypes = client.Settings.GetTranTypes();
            CollectionAssert.IsNotEmpty(tranTypes);
        }
    }
}