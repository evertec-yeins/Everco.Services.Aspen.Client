// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Settings.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 16:01 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Entities.Autonomous;
    using Internals;
    using Modules.Autonomous;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : ITokenModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones para la generación y validación de tokens o claves transnacionales.
        /// </summary>
        public ITokenModule Token => this;

        /// <summary>
        /// Obtiene la lista de canales para los que se pueden generar tokens o claves transaccionales.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="TokenChannelInfo" /> con los canales soportados para generar tokens transaccionales.
        /// </returns>
        public IList<TokenChannelInfo> GetChannels()
        {
            List<TokenChannelInfo> tokenChannels = CacheStore.Get<List<TokenChannelInfo>>(CacheKeys.TokenChannels);

            if (tokenChannels != null)
            {
                return tokenChannels;
            }

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TokenChannels);
            tokenChannels = this.Execute<List<TokenChannelInfo>>(request);
            CacheStore.Add(CacheKeys.TokenChannels, tokenChannels);
            return tokenChannels;
        }

        /// <summary>
        /// Obtiene la lista de canales para los que se pueden generar tokens o claves transaccionales.
        /// </summary>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<IList<TokenChannelInfo>> GetChannelsAsync()
        {
            return await Task.Run(this.GetChannels);
        }

        /// <summary>
        /// Obtiene la información de un token o clave transaccional en formato imagen (base64).
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Cadena de texto que representa a la imagen del token transaccional en formato base64.
        /// </returns>
        public string GetTokenByAlias(string channelId, string enrollmentAlias)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.GenerateTokenByAlias, endpointParameters);
            return this.Execute(request).Content;
        }

        /// <summary>
        /// Obtiene la información de un token o clave transaccional en formato imagen (base64).
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Instancia de <see cref="Task{TResult}" /> que representa el estado de la ejecución de la tarea.
        /// </returns>
        public async Task<string> GetTokenByAliasAsync(string channelId, string enrollmentAlias)
        {
            return await Task.Run(() => this.GetTokenByAlias(channelId, enrollmentAlias));
        }

        /// <summary>
        /// Valida la información de un token o clave transaccional generado y marca el token como utilizado.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="token">El valor del token que se desea redimir.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>
        /// La información resultante del proceso de redención del token transaccional.
        /// </returns>
        public TokenRedeemedInfo RedeemToken(
            string docType,
            string docNumber,
            string token,
            int amount,
            string metadata = null)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "docType", docType },
                { "docNumber", docNumber },
                { "amount", amount },
                { "metadata", metadata }
            };

            EndpointParameters endpointParameters = new EndpointParameters()
                .AddToken(token);

            IRestRequest request = new AspenRequest(
                Scope.Autonomous,
                EndpointMapping.RedeemToken,
                body,
                endpointParameters);

            return this.Execute<TokenRedeemedInfo>(request);
        }

        /// <summary>
        /// Valida la información de un token o clave transaccional generado y marca el token como utilizado.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="token">El valor del token que se desea redimir.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>
        /// La información resultante del proceso de redención del token transaccional.
        /// </returns>
        public async Task<TokenRedeemedInfo> RedeemTokenAsync(
            string docType,
            string docNumber,
            string token,
            int amount,
            string metadata = null)
        {
            return await Task.Run(() => this.RedeemToken(docType, docNumber, token, amount, metadata));
        }

        /// <summary>
        /// Genera y envía un token o clave transaccional a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>
        /// Diccionario de datos dinámicos generados por la respuesta.
        /// </returns>
        public DeliveredTokenCustomData SendToken(
            string docType,
            string docNumber,
            string accountType = null,
            int? amount = null,
            string metadata = null,
            Dictionary<string, string> tags = null)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "docType", docType },
                { "docNumber", docNumber },
                { "accountType", accountType },
                { "amount", amount },
                { "metadata", metadata },
                { "tags", tags }
            };

            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.SendToken, body);
            return this.Execute<DeliveredTokenCustomData>(request);
        }

        /// <summary>
        /// Genera y envía un token o clave transaccional a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que se solicita la generación de un token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>
        /// Diccionario de datos dinámicos generados por la respuesta.
        /// </returns>
        public async Task<DeliveredTokenCustomData> SendTokenAsync(
            string docType,
            string docNumber,
            string accountType = null,
            int? amount = null,
            string metadata = null,
            Dictionary<string, string> tags = null)
        {
            return await Task.Run(() => this.SendToken(docType, docNumber, accountType, amount, metadata, tags));
        }

        /// <summary>
        /// Solicita la anulación o cancelación de un token o clave transaccional.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario para el que fue emitido token transaccional.</param>
        /// <param name="docNumber">El número de documento del usuario para el que fue emitido token transaccional..</param>
        /// <param name="token">El valor del token que se desea cancelar.</param>
        /// <param name="appId">El identificador de la aplicación que emitió el token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta que fue asociado al token transaccional.</param>
        /// <param name="amount">El valor de la transacción asociado al token que se desea cancelar.</param>
        /// <param name="channelKey">El código el canal por el que se redime el token transaccional.</param>
        /// <param name="metadata">Los metadatos personalizados que fueron asociados en el proceso de generación del token transaccional.</param>
        /// <remarks>Esta operación solo está disponible cuando el servicio ASPEN fue iniciado en modo de depuración o para ambiente de desarrollo.</remarks>
        internal void NullifyToken(
            string docType,
            string docNumber,
            string token,
            int appId,
            string accountType = null,
            int? amount = null,
            string channelKey = null,
            string metadata = null)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "docType", docType },
                { "docNumber", docNumber },
                { "appId", appId },
                { "accountType", accountType },
                { "amount", amount },
                { "channelKey", channelKey },
                { "metadata", metadata }
            };

            EndpointParameters endpointParameters = new EndpointParameters()
                .AddToken(token);

            IRestRequest request = new AspenRequest(
                Scope.Autonomous,
                EndpointMapping.NullifyToken,
                body,
                endpointParameters);

            this.Execute(request);
        }
    }
}