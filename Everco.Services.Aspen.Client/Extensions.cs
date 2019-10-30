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
    using System.ComponentModel;
    using System.Reflection;
    using Internals;
    using RestSharp;

    /// <summary>
    /// Expone métdos de extensión para varios tipos.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Obtiene la información de mapeo a donde se debe enviar una solicitud al servicio Aspen.
        /// </summary>
        /// <param name="mapping">Campo en donde se busca la información de mapeo.</param>
        /// <param name="scope">Alcance de la aplicación que solicita la información.</param>
        /// <returns>Instancia de KeyValuePair&lt;System.String, Method&gt; con los valores extraidos del campo.</returns>
        /// <exception cref="InvalidOperationException">Missing EndPointMappingInfoAttribute for {mapping}</exception>
        public static KeyValuePair<string, Method> GetEndPointMappingInfo(this EndpointMapping mapping, Scope scope)
        {
            string keyName = $"GetEndPointMappingInfo.{scope}.{mapping}";
            object currentValue = AppDomain.CurrentDomain.GetData(keyName);
            if (currentValue != null)
            {
                return (KeyValuePair<string, Method>)currentValue;
            }

            EndPointMappingInfoAttribute attribute = typeof(EndpointMapping).GetField(mapping.ToString()).GetCustomAttribute<EndPointMappingInfoAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException($"Missing EndPointMappingInfoAttribute for {mapping}");
            }

            string prefix = string.Empty;
            switch (scope)
            {
                case Scope.Autonomous:
                    prefix = "/app";
                    break;

                case Scope.Delegated:
                    prefix = "/me";
                    break;
            }

            string resource = $"{prefix}/{attribute.Url.TrimStart('/').TrimEnd('/')}";
            KeyValuePair<string, Method> kvp = new KeyValuePair<string, Method>(resource, attribute.Method);
            AppDomain.CurrentDomain.SetData(keyName, kvp);
            return kvp;
        }

        /// <summary>
        /// Convierte el valor en <paramref name="input" /> al tipo especificado en <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">El tipo al que se convierte el valor en <paramref name="input" />.</typeparam>
        /// <param name="input">Texto (entrada) que se desea convertir.</param>
        /// <param name="defaultValue">valor de retorno si la conversión falla.</param>
        /// <returns>
        /// Instancia de <typeparamref name="T"/> o <paramref name="defaultValue" /> si la conversión falla.
        /// </returns>
        public static T Convert<T>(this string input, T defaultValue = default)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(input);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retorna <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío.
        /// </summary>
        /// <param name="input">Texto a evaluar.</param>
        /// <param name="defaultValue">Texto de reemplazo o  <see langword="null" /> para utilizar el texto predeterminado.</param>
        /// <returns>
        /// <paramref name="defaultValue" /> si <paramref name="input" /> es nulo o vacío; de lo contrario<paramref name="input" />.
        /// </returns>
        internal static string DefaultIfNullOrEmpty(
            this string input,
            string defaultValue = null) => string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }
}