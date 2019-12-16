// -----------------------------------------------------------------------
// <copyright file="IAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Identity;

    /// <summary>
    /// Define las operaciones que permiten establecer la identidad de la aplicación solicitante.
    /// </summary>
    /// <typeparam name="TFluent">El tipo de aplicación de retorno.</typeparam>
    public interface IAppIdentity<out TFluent>
    {
        /// <summary>
        /// Establece la identidad de la aplicación solicitante.
        /// </summary>
        /// <param name="apiKey">El ApiKey que identifica a la aplicación que se requiere para autenticarse en el servicio.</param>
        /// <param name="apiSecret">El ApiSecret asociado a la aplicación que se requiere para autenticarse en el servicio.</param>
        /// <returns>Instancia de <typeparamref name="TFluent"/> que permite interactuar con el servicio Aspen.</returns>
        TFluent WithIdentity(string apiKey, string apiSecret);

        /// <summary>
        /// Establece la identidad de la aplicación solicitante.
        /// </summary>
        /// <param name="appIdentity">Identidad de la aplicación solicitante.</param>
        /// <returns>Instancia de <typeparamref name="TFluent"/> uue permite interactuar con el servicio Aspen.</returns>
        TFluent WithIdentity(IAppIdentity appIdentity);
    }
}