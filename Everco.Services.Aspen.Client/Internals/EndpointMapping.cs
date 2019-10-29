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
        Signin
    }
}