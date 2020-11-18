// -----------------------------------------------------------------------
// <copyright file="ITokenModule.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 16:03 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Everco.Services.Aspen.Entities.Autonomous;

    /// <summary>
    /// Define las operaciones soportadas para la generación y validación de tokens o claves transaccionales.
    /// </summary>
    public interface ITokenModule
    {
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
        /// Obtiene la información de un token o clave transaccional en formato imagen (base64).
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Cadena de texto que representa a la imagen del token transaccional en formato base64.
        /// </returns>
        string GetTokenByAlias(string channelId, string enrollmentAlias);

        /// <summary>
        /// Obtiene la información de un token o clave transaccional en formato imagen (base64).
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        Task<string> GetTokenByAliasAsync(string channelId, string enrollmentAlias);

        /// <summary>
        /// Valida la información de un token o clave transaccional generado y marca el token como utilizado.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="token">El valor del token que se desea redimir.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>La información resultante del proceso de redención del token transaccional.</returns>
        TokenRedeemedInfo RedeemToken(
            string docType,
            string docNumber,
            string token,
            int amount,
            string metadata = null);

        /// <summary>
        /// Valida la información de un token o clave transaccional generado y marca el token como utilizado.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="token">El valor del token que se desea redimir.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>La información resultante del proceso de redención del token transaccional.</returns>
        Task<TokenRedeemedInfo> RedeemTokenAsync(
            string docType,
            string docNumber,
            string token,
            int amount,
            string metadata = null);

        /// <summary>
        /// Genera y envía un token o clave transaccional a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>Diccionario de datos dinámicos generados por la respuesta.</returns>
        DeliveredTokenCustomData SendToken(
            string docType,
            string docNumber,
            string accountType = null,
            int? amount = null,
            string metadata = null,
            Dictionary<string, string> tags = null);

        /// <summary>
        /// Genera y envía un token o clave transaccional a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>Diccionario de datos dinámicos generados por la respuesta.</returns>
        Task<DeliveredTokenCustomData> SendTokenAsync(
            string docType,
            string docNumber,
            string accountType = null,
            int? amount = null,
            string metadata = null,
            Dictionary<string, string> tags = null);
    }
}