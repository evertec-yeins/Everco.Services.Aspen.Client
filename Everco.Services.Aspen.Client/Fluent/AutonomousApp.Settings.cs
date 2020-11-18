// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Settings.cs" company="Evertec Colombia">
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
    using Modules.Autonomous;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : ISettingsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public ISettingsModule Settings => this;

        /// <summary>
        /// Obtiene la lista de operadores de telefonía móvil soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="CarrierInfo" /> con los operadores de telefonía soportados.
        /// </returns>
        public IList<CarrierInfo> GetCarriers()
        {
            List<CarrierInfo> carriers = CacheStore.Get<List<CarrierInfo>>(CacheKeys.Carriers);

            if (carriers != null)
            {
                return carriers;
            }

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.Carriers);
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
        /// Obtiene la lista de tipos de documento soportados para la aplicación.
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

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.DocTypes);
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
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
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
            
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.PaymentTypes);
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
        /// Obtiene los valores admitidos de recarga por operador soportados para la aplicación.
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

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TopUpValues);
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
        /// Obtiene la lista de los tipos de transacción soportados para la aplicación.
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

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TranTypes);
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
