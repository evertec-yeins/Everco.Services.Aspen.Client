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
    using System.Threading.Tasks;
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
            List<CarrierInfo> carriers = CacheStore.Get<List<CarrierInfo>>(CacheKeys.Carriers);

            if (carriers != null)
            {
                return carriers;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.Carriers);
            carriers = this.Execute<List<CarrierInfo>>(request);
            CacheStore.Add(CacheKeys.Carriers, carriers);
            return carriers;
        }

        /// <summary>
        /// Obtiene la lista de operadores de telefonía móvil soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<CarrierInfo>> GetCarriersAsync()
        {
            return await Task.Run(this.GetCarriers);
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento soportados.
        /// </returns>
        public IList<DocTypeInfo> GetDocTypes()
        {
            List<DocTypeInfo> docTypes = CacheStore.Get<List<DocTypeInfo>>(CacheKeys.DocTypes);

            if (docTypes != null)
            {
                return docTypes;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.DocTypes);
            docTypes = this.Execute<List<DocTypeInfo>>(request);
            CacheStore.Add(CacheKeys.DocTypes, docTypes);
            return docTypes;
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<DocTypeInfo>> GetDocTypesAsync()
        {
            return await Task.Run(this.GetDocTypes);
        }

        /// <summary>
        /// Obtiene la lista de opciones que representan el menú de una aplicación móvil.
        /// </summary>
        /// <returns>Lista de opciones de menú.</returns>
        public IList<MenuItemInfo> GetMenu()
        {
            List<MenuItemInfo> menuItems = CacheStore.Get<List<MenuItemInfo>>(CacheKeys.MenuItems);

            if (menuItems != null)
            {
                return menuItems;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.MenuItems);
            menuItems = this.Execute<List<MenuItemInfo>>(request);
            CacheStore.Add(CacheKeys.MenuItems, menuItems);
            return menuItems;
        }

        /// <summary>
        /// Obtiene la lista de opciones que representan el menú de la aplicación móvil.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<MenuItemInfo>> GetMenuAsync()
        {
            return await Task.Run(this.GetMenu);
        }

        /// <summary>
        /// Obtiene la configuración de valores misceláneos soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Colección de valores admitidos.
        /// </returns>
        public MiscellaneousSettings GetMiscellaneousSettings()
        {
            MiscellaneousSettings miscellaneousSettings = CacheStore.Get<MiscellaneousSettings>(CacheKeys.MiscellaneousSettings);

            if (miscellaneousSettings != null)
            {
                return miscellaneousSettings;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.MiscellaneousSettings);
            miscellaneousSettings = this.Execute<MiscellaneousSettings>(request);
            CacheStore.Add(CacheKeys.MiscellaneousSettings, miscellaneousSettings);
            return miscellaneousSettings;
        }

        /// <summary>
        /// Obtiene la configuración de valores misceláneos soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<MiscellaneousSettings> GetMiscellaneousSettingsAsync()
        {
            return await Task.Run(this.GetMiscellaneousSettings);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="PaymentTypeInfo" /> con los tipos de pago para la aplicación solicitante.
        /// </returns>
        public IList<PaymentTypeInfo> GetPaymentTypes()
        {
            List<PaymentTypeInfo> paymentTypes = CacheStore.Get<List<PaymentTypeInfo>>(CacheKeys.PaymentTypes);

            if (paymentTypes != null)
            {
                return paymentTypes;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.PaymentTypes);
            paymentTypes = this.Execute<List<PaymentTypeInfo>>(request);
            CacheStore.Add(CacheKeys.PaymentTypes, paymentTypes);
            return paymentTypes;
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<PaymentTypeInfo>> GetPaymentTypesAsync()
        {
            return await Task.Run(this.GetPaymentTypes);
        }

        /// <summary>
        /// Obtiene los valores admitidos de recarga por operador para la aplicación solicitante
        /// </summary>
        /// <returns>
        /// Lista de <see cref="TopUpInfo" /> con los valores admitidos de recarga por operador para la aplicación solicitante.
        /// </returns>
        public IList<TopUpInfo> GetTopUpValues()
        {
            List<TopUpInfo> topUpValues = CacheStore.Get<List<TopUpInfo>>(CacheKeys.TopUpValues);

            if (topUpValues != null)
            {
                return topUpValues;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.TopUpValues);
            topUpValues = this.Execute<List<TopUpInfo>>(request);
            CacheStore.Add(CacheKeys.TopUpValues, topUpValues);
            return topUpValues;
        }

        /// <summary>
        /// Obtiene los valores admitidos de recarga por operador soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<TopUpInfo>> GetTopUpValuesAsync()
        {
            return await Task.Run(this.GetTopUpValues);
        }

        /// <summary>
        /// Obtiene la lista de los tipos de transacción para una aplicación.
        /// </summary>
        /// <returns>
        /// Lista de tipos de transacción soportados.
        /// </returns>
        public IList<TranTypeInfo> GetTranTypes()
        {
            List<TranTypeInfo> tranTypes = CacheStore.Get<List<TranTypeInfo>>(CacheKeys.TranTypes);

            if (tranTypes != null)
            {
                return tranTypes;
            }

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.TranTypes);
            tranTypes = this.Execute<List<TranTypeInfo>>(request);
            CacheStore.Add(CacheKeys.TranTypes, tranTypes);
            return tranTypes;
        }

        /// <summary>
        /// Obtiene la lista de los tipos de transacción soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<TranTypeInfo>> GetTranTypesAsync()
        {
            return await Task.Run(this.GetTranTypes);
        }
    }
}
