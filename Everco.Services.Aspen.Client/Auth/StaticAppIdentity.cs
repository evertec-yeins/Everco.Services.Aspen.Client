// -----------------------------------------------------------------------
// <copyright file="StaticAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Auth
{
    using Identity;
    using Internals;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen.
    /// </summary>
    internal class StaticAppIdentity : IAppIdentity
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="StaticAppIdentity"/>
        /// </summary>
        /// <param name="apiKey">El ApiKey de la aplicación que se usará para firmar la autenticación.</param>
        /// <param name="apiSecret">El ApiSecret asociado a la aplicación que se usará para la autenticación</param>
        public StaticAppIdentity(string apiKey, string apiSecret)
        {
            Throw.IfNullOrEmpty(apiKey, nameof(apiKey));
            Throw.IfNullOrEmpty(apiSecret, nameof(apiSecret));
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