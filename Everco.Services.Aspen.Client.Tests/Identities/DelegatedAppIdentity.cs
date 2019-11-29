// -----------------------------------------------------------------------
// <copyright file="DelegatedAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using Everco.Services.Aspen.Client.Identity;

    /// <summary>
    /// Representa la información que se utiliza para autenticar la solicitud en el servicio Aspen.
    /// </summary>
    internal class DelegatedAppIdentity 
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity assistantIdentity;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static IAppIdentity masterIdentity;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DelegatedAppIdentity" />.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación.</param>
        /// <param name="apiSecret">El secreto de la aplicación.</param>
        public DelegatedAppIdentity(string apiKey, string apiSecret)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
        }

        /// <summary>
        /// Obtiene la identidad de una aplicación delegada para fines de comparación.
        /// </summary>
        public static IAppIdentity Helper =>
            assistantIdentity ?? (assistantIdentity = new EnvironmentIdentity(
                                      "ASPEN:DELEGATEDAPP2:APIKEY",
                                      "ASPEN:DELEGATEDAPP2:APISECRET"));

        /// <summary>
        /// Obtiene la identidad de una aplicación delegada para la autenticación de las pruebas automatizadas.
        /// </summary>
        public static IAppIdentity Master => masterIdentity ?? (masterIdentity = new EnvironmentIdentity(
                                                 "ASPEN:DELEGATEDAPP1:APIKEY",
                                                 "ASPEN:DELEGATEDAPP1:APISECRET"));

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