// -----------------------------------------------------------------------
// <copyright file="EndpointMapping.cs" company="Evertec Colombia">
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
        /// Operación para obtener los valores de configuración de la aplicación.
        /// </summary>
        [EndPointMappingInfo("/appMov/settings", Method.GET)]
        AppMovSettings,

        /// <summary>
        /// Operación para obtener las cuentas asociadas a un usuario a partir del documento de identidad.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]", Method.GET)]
        AccountsByUserIdentity,

        /// <summary>
        /// Operación para obtener las cuentas asociadas a un usuario a partir alias de registro.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/channel/@[ChannelId]/alias/@[EnrollmentAlias]", Method.GET)]
        AccountsByAlias,

        /// <summary>
        /// Operación para obtener las cuentas asociadas al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts", Method.GET)]
        AccountsFromCurrentUser,

        /// <summary>
        /// Operación para obtener los saldos de una cuenta asociada a un usuario.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]/@[AccountId]/balances", Method.GET)]
        BalancesByUserIdentity,

        /// <summary>
        /// Operación para obtener los saldos de una cuenta asociada a un usuario a partir alias de registro.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/channel/@[ChannelId]/alias/@[EnrollmentAlias]/id/@[AccountId]/balances", Method.GET)]
        BalancesByAlias,

        /// <summary>
        /// Operación para obtener los saldos de una cuenta asociada al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[AccountId]/balances", Method.GET)]
        BalancesFromCurrentUser,

        /// <summary>
        /// Operación para obtener los movimientos de una cuenta asociada a un usuario.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[DocType]/@[DocNumber]/@[AccountId]/@[AccountTypeId]/statements", Method.GET)]
        StatementsByUserIdentity,

        /// <summary>
        /// Operación para obtener los movimientos de una cuenta asociada al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/@[AccountId]/@[AccountTypeId]/statements", Method.GET)]
        StatementsFromCurrentUser,

        /// <summary>
        /// Operación para obtener los movimientos de una cuenta asociada a un usuario a partir alias de registro.
        /// </summary>
        [EndPointMappingInfo("/inquires/accounts/channel/@[ChannelId]/alias/@[EnrollmentAlias]/id/@[AccountId]/type/@[AccountTypeId]/statements", Method.GET)]
        StatementsByAlias,

        /// <summary>
        /// Operación para obtener las cuentas para transferencia de saldos vinculadas a un usuario.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts/docType/@[OwnerDocType]/docNumber/@[OwnerDocNumber]", Method.GET)]
        TransferAccountsByUserIdentity,

        /// <summary>
        /// Operación para vincular una cuenta para transferencia de saldos a un usuario.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts/docType/@[OwnerDocType]/docNumber/@[OwnerDocNumber]", Method.POST)]
        LinkTransferAccountByUserIdentity,

        /// <summary>
        /// Operación para desvincular una cuenta registrada a un usuario por el alias de la cuenta.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts/docType/@[OwnerDocType]/docNumber/@[OwnerDocNumber]/alias/@[Alias]", Method.DELETE)]
        UnlinkTransferAccountByUserIdentityAndAlias,

        /// <summary>
        /// Operación para desvincular una cuenta registrada a un usuario por la identificación del titular de la cuenta.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts/docTypeOwner/@[OwnerDocType]/docNumberOwner/@[OwnerDocNumber]/docTypeLink/@[CardHolderDocType]/docNumberLink/@[CardHolderDocNumber]", Method.DELETE)]
        UnlinkTransferAccountByUserIdentityAndCardHolder,

        /// <summary>
        /// Operación para obtener las cuentas para transferencia de saldos vinculadas al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts", Method.GET)]
        TransferAccountsFromCurrentUser,

        /// <summary>
        /// Operación para vincular una cuenta para transferencia de saldos al usuario actual autenticado.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts", Method.POST)]
        LinkTransferAccountFromCurrentUser,

        /// <summary>
        /// Operación para desvincular una cuenta registrada al usuario actual autenticado por el alias de la cuenta.
        /// </summary>
        [EndPointMappingInfo("/transfers/accounts/@[Alias]", Method.DELETE)]
        UnlinkTransferAccountFromCurrentUserByAlias
    }
}