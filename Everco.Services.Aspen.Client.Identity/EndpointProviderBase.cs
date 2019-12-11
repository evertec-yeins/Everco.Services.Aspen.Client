// -----------------------------------------------------------------------
// <copyright file="EndpointProviderBase.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-10 03:03 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Clase base para las implementaciones de <see cref="IEndpointProvider"/>
    /// </summary>
    public abstract class EndpointProviderBase : IEndpointProvider
    {
        /// <summary>
        /// Obtiene la URL base para las solicitudes hacia al API de ASPEN.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Obtiene el tiempo de espera para las respuestas del servicio Aspen.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Establece el valor de la propiedad <see cref="Url"/>.
        /// </summary>
        /// <param name="value">Representación del valor que se desea establecer.</param>
        /// <param name="defaultIfMissing">Valor predeterminado si <paramref name="value"/> fuera nulo o vacío.</param>
        protected void SetUrl(string value, string defaultIfMissing = "https://localhost")
        {
            if (string.IsNullOrWhiteSpace(value.TryDecrypt()))
            {
                value = defaultIfMissing;
            }

            try
            {
                Uri uri = new Uri(value, UriKind.Absolute);
                this.Url = uri.AbsoluteUri;
            }
            catch (SystemException exception)
            {
                string message = $"{this.Url} is an invalid Uniform Resource Identifier.";
                throw new UriFormatException(message, exception);
            }
        }

        /// <summary>
        /// Establece el valor de la propiedad <see cref="Timeout"/>.
        /// </summary>
        /// <param name="value">Representación del valor que se desea establecer.</param>
        /// <param name="variableName">Nombre de la variable de donde se leyo el valor a establecer.</param>
        /// <param name="defaultIfMissing">Valor predeterminado si <paramref name="value"/> fuera nulo o vacío.</param>
        protected void SetTimeout(string value, string variableName, string defaultIfMissing = "00:00:15")
        {
            if (string.IsNullOrWhiteSpace(value.TryDecrypt()))
            {
                value = defaultIfMissing;
            }

            if (!TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out TimeSpan result))
            {
                string message = $"{variableName}:{value} is not recognized as a TimeSpan value";
                throw new FormatException(message);
            }

            this.Timeout = result;
        }
    }
}