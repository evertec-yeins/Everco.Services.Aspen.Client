// -----------------------------------------------------------------------
// <copyright file="NullWebProxy.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:25 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;
    using System.Net;

    /// <summary>
    /// Representa una instancia de un servidor proxy sin configurar o con valores nulos.
    /// </summary>
    public class NullWebProxy : IWebProxy
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NullWebProxy"/>
        /// </summary>
        public NullWebProxy()
        {
            this.Credentials = null;
        }

        /// <summary>
        /// Las credenciales para enviar al servidor proxy para la autenticación.
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Devuelve el URI del proxy.
        /// </summary>
        /// <param name="destination">Instancia de <see cref="T:System.Uri"></see> que especifica el recurso de Internet solicitado.</param>
        /// <returns>Instancia de <see cref="Uri"></see> instancia que contiene el URI del proxy utilizado para contactar con <paramref name="destination"></paramref>.</returns>
        public Uri GetProxy(Uri destination)
        {
            return null;
        }

        /// <summary>
        /// Indica que el proxy no debe usarse para el host especificado.
        /// </summary>
        /// <param name="host">La <see cref="T:System.Uri"></see> del host para verificar el uso del proxy.</param>
        /// <returns>Siempre false en esta instancia.</returns>
        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}