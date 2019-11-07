// -----------------------------------------------------------------------
// <copyright file="Delegated.Settings.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las entidades de información relacionadas con parametrización del sistema para una aplicación de alcance de delegada.
    /// </summary>
    [TestFixture]
    public class DelegatedSettingsTests
    {
        /// <summary>
        /// Proporciona un conjunto común de funciones que se ejecutarán antes de llamar a cada método de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Obtener los tipos de documento de la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetDocTypesWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        /// <summary>
        /// Obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetPaymentTypesWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<PaymentTypeInfo> paymentTypes = client.Settings.GetPaymentTypes();
            CollectionAssert.IsNotEmpty(paymentTypes);
        }

        /// <summary>
        /// Obtiener los valores admitidos de recarga por operador para la aplicación solicitante funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetTopUpValuesWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<TopUpInfo> topUpValues = client.Settings.GetTopUpValues();
            CollectionAssert.IsNotEmpty(topUpValues);
        }

        /// <summary>
        /// Obtener la lista de los tipos de transacción soportados para la aplicación solicitante funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetTranTypesWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<TranTypeInfo> tranTypes = client.Settings.GetTranTypes();
            CollectionAssert.IsNotEmpty(tranTypes);
        }

        /// <summary>
        /// Obtener la lista de opciones que representan el menú para la aplicación móvil funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetAppMenuItemsWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            IList<MenuItemInfo> menuItems = client.Settings.GetMenu();
            CollectionAssert.IsNotEmpty(menuItems);
        }

        /// <summary>
        /// Obtener la configuración de valores misceláneos soportados para la aplicación funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Settings")]
        public void GetMiscellaneousSettingsWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(UserIdentity.Default)
                .GetClient();

            MiscellaneousSettings miscellaneous = client.Settings.GetMiscellaneousSettings();
            CollectionAssert.IsNotEmpty(miscellaneous);
        }
    }
}