// -----------------------------------------------------------------------
// <copyright file="ISession.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Auth;

    /// <summary>
    /// Define operaciones y propiedades para acceder a los valores actuales de conexión con el servicio Aspen.
    /// </summary>
    /// <typeparam name="TFluent">El tipo de aplicación de retorno.</typeparam>
    public interface ISession<out TFluent>
    {
        /// <summary>
        /// Obtiene el token de autenticación emitido para la sesión.
        /// </summary>
        IAuthToken AuthToken { get; }

        /// <summary>
        /// Obtiene la instancia actual configurada para interactuar con el servicio Aspen.
        /// </summary>
        /// <returns>Instancia de <typeparamref name="TFluent"/> que permite interactuar con el servicio Aspen.</returns>
        TFluent GetClient();
    }
}