// -----------------------------------------------------------------------
// <copyright file="ISettingsModule.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las operaciones soportadas apara acceder a entidades de información relacionadas con parametrización del sistema.
    /// </summary>
    public interface ISettingsModule
    {
        /// <summary>
        /// Obtiene la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        /// <returns>Lista de tipos de documento soportados.</returns>
        IList<DocTypeInfo> GetDocTypes();

        /// <summary>
        /// Obtiene la lista de operadores de telefonía móvil soportados para la aplicación.
        /// </summary>
        /// <returns>Lista de operadores de telefonía soportados.</returns>
        IList<CarrierInfo> GetCarriers();

        /// <summary>
        /// Obtiene la lista de los tipos de transacción soportados para la aplicación.
        /// </summary>
        /// <returns>Lista de tipos de transacción soportados.</returns>
        IList<TranTypeInfo> GetTranTypes();

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
        /// </summary>
        /// <returns>Lista de <see cref="PaymentTypeInfo"/> con los tipos de pago para la aplicación solicitante.</returns>
        IList<PaymentTypeInfo> GetPaymentTypes();

        /// <summary>
        /// Obtiene los valores admitidos de recarga por operador soportados para la aplicación.
        /// </summary>
        /// <returns>Lista de <see cref="TopUpInfo"/> con los valores admitidos de recarga por operador para la aplicación solicitante.</returns>
        IList<TopUpInfo> GetTopUpValues();
    }
}