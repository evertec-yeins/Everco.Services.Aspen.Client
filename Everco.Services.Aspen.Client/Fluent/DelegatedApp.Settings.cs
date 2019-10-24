// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Settings.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using Entities;
    using Internals;
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance delegada.
    /// </summary>
    public sealed partial class DelegatedApp : ISettingsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public ISettingsModule Settings => this;

        /// <summary>
        /// Obtiene la lista de operadores de telefonía móvil soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de operadores de telefonía soportados.
        /// </returns>
        public IList<CarrierInfo> GetCarriers()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.Telcos}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<CarrierInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento soportados.
        /// </returns>
        public IList<DocTypeInfo> GetDocTypes()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.DocTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<DocTypeInfo>>(request);
        }
        /// <summary>
        /// Obtiene la lista de opciones que representan el menú de una aplicación móvil.
        /// </summary>
        /// <returns>Lista de opciones de menú.</returns>
        public IList<MenuItemInfo> GetMenu()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.Menu}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<MenuItemInfo>>(request);
        }

        /// <summary>
        /// Obtiene la configuración de valores misceláneos soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Colección de valores admitidos.
        /// </returns>
        public MiscellaneousSettings GetMiscellaneousSettings()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.Miscellaneous}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<MiscellaneousSettings>(request);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="PaymentTypeInfo" /> con los tipos de pago para la aplicación solicitante.
        /// </returns>
        public IList<PaymentTypeInfo> GetPaymentTypes()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.PaymentTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<PaymentTypeInfo>>(request);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="TopUpInfo" /> con los valores admitidos de recarga por operador para la aplicación solicitante.
        /// </returns>
        public IList<TopUpInfo> GetTopUpValues()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.TopUp}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<TopUpInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de los tipos de transacción para una aplicación.
        /// </summary>
        /// <returns>
        /// Lista de tipos de transacción soportados.
        /// </returns>
        public IList<TranTypeInfo> GetTranTypes()
        {
            string resource = $"{Routes.DelegatedRoot}{Routes.Resources.TranTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<TranTypeInfo>>(request);
        }
    }
}
