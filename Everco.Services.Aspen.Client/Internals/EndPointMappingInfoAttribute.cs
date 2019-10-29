// -----------------------------------------------------------------------
// <copyright file="EndPointMappingInfoAttribute.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-24 10:58 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Everco.Services.Aspen.Client.Internals
{
    using System;
    using RestSharp;

    /// <summary>
    /// Representa la información de mapeo de un endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EndPointMappingInfoAttribute : Attribute
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EndPointMappingInfoAttribute"/>
        /// </summary>
        /// <param name="url">Url del endpoint a donde se deben enviar la solicitud.</param>
        /// <param name="method">Método Http que se utiliza al enviar la solicitud al endpoint.</param>
        public EndPointMappingInfoAttribute(string url, Method method)
        {
            this.Url = url;
            this.Method = method;
        }

        /// <summary>
        /// Obtiene la Url del endpoint a donde se deben enviar la solicitud.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Obtiene el método Http que se utiliza al enviar la solicitud al endpoint.
        /// </summary>
        public Method Method { get; private set; }
    }
}