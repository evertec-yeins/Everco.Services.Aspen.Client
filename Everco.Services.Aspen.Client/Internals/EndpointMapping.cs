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
        /// Operación para cifrar una cadena de texto usando el algoritmo de cifrado del servicio Aspen.
        /// </summary>
        [EndPointMappingInfo("/utils/crypto", Method.POST)]
        Encrypt,

        /// <summary>
        /// Operación para obtener la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        [EndPointMappingInfo("/utils/document-types", Method.GET)]
        GetDefaultDocTypes,

        /// <summary>
        /// Operación para registrar la información de las excepciones que se produzcan por cierres inesperados de una aplicación.
        /// </summary>
        [EndPointMappingInfo("/utils/app/crash", Method.POST)]
        SaveAppCrash,

        /// <summary>
        /// Operación para obtener la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/document-types", Method.GET)]
        GetDocTypes,

        /// <summary>
        /// Operación para obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
        /// </summary>
        [EndPointMappingInfo("/resx/payment-types", Method.GET)]
        GetPaymentTypes,
    }
}