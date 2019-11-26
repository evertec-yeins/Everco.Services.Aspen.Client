// -----------------------------------------------------------------------
// <copyright file="HardCodeIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 12:47 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;

    /// <summary>
    /// Obtiene la información que se utiliza para autenticar la solicitud en el servicio Aspen a partir de los valores proporcionados en el constructor de la clase.
    /// </summary>
    [Obsolete("No es recomendable utilizar esta clase. En su lugar utilice cualquier clase de las que implementa la interfaz IAppIdentity")]
    public class HardCodeIdentity : IAppIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HardCodeIdentity"/>
        /// </summary>
        /// <param name="apiKey">Valor que se establece como ApiKey.</param>
        /// <param name="apiSecret">Valor que se establece como ApiSecret.</param>
        public HardCodeIdentity(string apiKey, string apiSecret)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
        }

        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        public string ApiSecret { get; }
    }
}