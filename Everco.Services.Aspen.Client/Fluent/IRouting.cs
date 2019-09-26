// -----------------------------------------------------------------------
// <copyright file="IRouting.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Providers;

    /// <summary>
    /// Define las operaciones que permiten establecer la Url del servicio Aspen y tiempo de espera para las respuestas.
    /// </summary>
    /// <typeparam name="TFluent">El tipo de aplicación de retorno.</typeparam>
    public interface IRouting<out TFluent>
    {
        /// <summary>
        /// Establece la Url base para las solicitudes al servicio Aspen.
        /// </summary>
        /// <param name="endpointProvider">Instancia con la configuración del servicio Aspen.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        IAppIdentity<TFluent> RoutingTo(IEndpointProvider endpointProvider);

        /// <summary>
        /// Establece la Url base para las solicitudes al servicio Aspen.
        /// </summary>
        /// <param name="baseUrl">La Url base para las solicitudes realizadas hacia al servicio Aspen. Ejemplo: <a>http://localhost/api</a></param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio. Valor predeterminado: 15 segundos.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        IAppIdentity<TFluent> RoutingTo(string baseUrl, int? timeout = null);
    }
}