// -----------------------------------------------------------------------
// <copyright file="EndpointMapping.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-24 10:57 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using RestSharp;

    /// <summary>
    /// Define el mapeo entre una operación, el endpoint a donde se envía y el método a utilizar.
    /// </summary>
    public enum EndpointMapping
    {
        /// <summary>
        /// Operación de generación de un token de autenticación.
        /// </summary>
        [EndPointMappingInfo("/auth/signin", Method.POST)]
        Signin,

        /// <summary>
        /// Operación de actualización del secreto de una aplicación.
        /// </summary>
        [EndPointMappingInfo("/auth/secret", Method.POST)]
        ApiSecret,

        /// <summary>
        /// Operación para cifrar una cadena de texto usando el algoritmo de cifrado del servicio.
        /// </summary>
        [EndPointMappingInfo("/utils/crypto", Method.POST)]
        Encrypt,

        /// <summary>
        /// Operación para obtener la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        [EndPointMappingInfo("/utils/document-types", Method.GET)]
        DefaultDocTypes,

        /// <summary>
        /// Operación para registrar la información de las excepciones que se produzcan por cierres inesperados de una aplicación.
        /// </summary>
        [EndPointMappingInfo("/utils/app/crash", Method.POST)]
        AppCrash,

        /// <summary>
        /// Operación para obtener la lista de tipos de documento soportados para una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/document-types", Method.GET)]
        DocTypes,

        /// <summary>
        /// Operación para obtener los tipos de pagos que se pueden realizar a una cuenta soportados para una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/payment-types", Method.GET)]
        PaymentTypes,

        /// <summary>
        /// Operación para obtener la lista de operadores de telefonía móvil soportados para una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/telcos", Method.GET)]
        Carriers,

        /// <summary>
        /// Operación para obtener los valores admitidos para los procesos de recargas a celular soportados para una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/topups", Method.GET)]
        TopUp,

        /// <summary>
        /// Operación para obtener los tipos de transacción soportados para una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/tran-types", Method.GET)]
        TranTypes,

        /// <summary>
        /// Operación para obtener las opciones de menú de una aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/menu", Method.GET)]
        MenuItems,

        /// <summary>
        /// Operación para obtener los valores misceláneos de la aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/miscs", Method.GET)]
        Miscellaneous,

        /// <summary>
        /// Operación para obtener las cuentas asociadas a un usuario.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]", Method.GET)]
        AccountsByUser,

        /// <summary>
        /// Operación para obtener las cuentas asociadas al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts", Method.GET)]
        AccountsFromCurrentUser,

        /// <summary>
        /// Operación para obtener los saldos de una cuenta asociada a un usuario.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]/@[AccountId]/balances", Method.GET)]
        BalancesByUser,

        /// <summary>
        /// Operación para obtener los saldos de una cuenta asociada al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[AccountId]/balances", Method.GET)]
        BalancesFromCurrentUser,

        /// <summary>
        /// Operación para obtener los movimientos de una cuenta asociada a un usuario.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]/@[AccountId]/@[AccountTypeId]/statements", Method.GET)]
        StatementsByUser,

        /// <summary>
        /// Operación para obtener los movimientos de una cuenta asociada al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[AccountId]/@[AccountTypeId]/statements", Method.GET)]
        StatementsFromCurrentUser,
    }
}