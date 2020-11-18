// -----------------------------------------------------------------------
// <copyright file="ITokenModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 16:03 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Delegated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Everco.Services.Aspen.Entities.Delegated;

    /// <summary>
    /// Define las operaciones soportadas para la generación y validación de tokens o claves transaccionales.
    /// </summary>
    public interface ITokenModule
    {
        /// <summary>
        /// Genera un token o clave transaccional para el usuario autenticado.
        /// </summary>
        /// <param name="pinNumber">El pin transaccional del usuario.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="channelKey">El código del canal para el que se emite el token transaccional.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>La información de un token transaccional generado para el usuario.</returns>
        TokenResponseInfo GenerateToken(
            string pinNumber,
            string accountType = null,
            int? amount = null,
            string channelKey = null,
            string metadata = null);

        /// <summary>
        /// Genera un token o clave transaccional para el usuario autenticado.
        /// </summary>
        /// <param name="pinNumber">El pin transaccional del usuario.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="channelKey">El código del canal para el que se emite el token transaccional.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>La información de un token transaccional generado para el usuario.</returns>
        Task<TokenResponseInfo> GenerateTokenAsync(
            string pinNumber,
            string accountType = null,
            int? amount = null,
            string channelKey = null,
            string metadata = null);

        /// <summary>
        /// Obtiene la lista de canales para los que se pueden generar tokens o claves transaccionales.
        /// </summary>
        /// <returns>Lista de <see cref="TokenChannelInfo"/> con los canales soportados para generar tokens transaccionales.</returns>
        IList<TokenChannelInfo> GetChannels();

        /// <summary>
        /// Obtiene la lista de canales para los que se pueden generar tokens o claves transaccionales.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<IList<TokenChannelInfo>> GetChannelsAsync();

        /// <summary>
        /// Genera y envía un token o clave transaccional al usuario autenticado.
        /// </summary>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>Diccionario de datos dinámicos generados por la respuesta.</returns>
        DeliveredTokenCustomData SendToken(
            string metadata = null,
            string accountType = null,
            int? amount = null,
            Dictionary<string, string> tags = null);

        /// <summary>
        /// Genera y envía un token o clave transaccional al usuario autenticado.
        /// </summary>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>Diccionario de datos dinámicos generados por la respuesta.</returns>
        Task<DeliveredTokenCustomData> SendTokenAsync(
            string metadata = null,
            string accountType = null,
            int? amount = null,
            Dictionary<string, string> tags = null);
    }
}