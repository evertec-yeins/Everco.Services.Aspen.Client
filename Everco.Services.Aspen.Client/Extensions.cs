// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Internals;
    using RestSharp;

    /// <summary>
    /// Expone métdos de extensión para varios tipos.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Retorna <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío.
        /// </summary>
        /// <param name="input">Texto a evaluar.</param>
        /// <param name="defaultValue">Texto de reemplazo o  <see langword="null" /> para utilizar el texto predeterminado.</param>
        /// <returns>
        /// <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío; de lo contrario<paramref name="input" />.
        /// </returns>
        internal static string DefaultIfNullOrEmpty(this string input, string defaultValue = null) => string.IsNullOrWhiteSpace(input) ? defaultValue : input;

        /// <summary>
        /// Obtiene la información de mapeo a donde se debe enviar una solicitud al servicio Aspen.
        /// </summary>
        /// <param name="mapping">Campo en donde se busca la información de mapeo.</param>
        /// <param name="scope">Alcance de la aplicación que solicita la información.</param>
        /// <returns>Instancia de KeyValuePair&lt;System.String, Method&gt; con los valores extraidos del campo.</returns>
        /// <exception cref="InvalidOperationException">Missing EndPointMappingInfoAttribute for {mapping}</exception>
        public static KeyValuePair<string, Method> GetEndPointMappingInfo(this EndpointMapping mapping, Scope scope)
        {
            string keyName = $"GetEndPointMappingInfo.{mapping}";
            object currentValue = AppDomain.CurrentDomain.GetData(keyName);
            if (currentValue != null)
            {
                return (KeyValuePair<string, Method>)currentValue;
            }

            EndPointMappingInfoAttribute attrib = typeof(EndpointMapping).GetField(mapping.ToString()).GetCustomAttribute<EndPointMappingInfoAttribute>();

            if (attrib == null)
            {
                throw new InvalidOperationException($"Missing EndPointMappingInfoAttribute for {mapping}");
            }

            string prefix = scope switch
            {
                Scope.Delegated => "/me",
                Scope.Autonomous => "/app",
                _ => string.Empty
            };

            string resource = $"{prefix}/{attrib.Url.TrimStart('/').TrimEnd('/')}";
            KeyValuePair<string, Method> kvp = new KeyValuePair<string, Method>(resource, attrib.Method);
            AppDomain.CurrentDomain.SetData(keyName, kvp);
            return kvp;
        }
    }
}