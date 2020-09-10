// -----------------------------------------------------------------------
// <copyright file="DelegatedApp.Token.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-09-08 17:00 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Entities.Delegated;
    using Internals;
    using Modules.Delegated;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance delegada.
    /// </summary>
    public sealed partial class DelegatedApp : ITokenModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones para la generación y validación de tokens o claves transnacionales.
        /// </summary>
        public ITokenModule Token => this;

        /// <summary>
        /// Genera un token o clave transaccional para el usuario autenticado.
        /// </summary>
        /// <param name="pinNumber">El pin transaccional del usuario.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="channelKey">El código del canal para el que se emite el token transaccional.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>
        /// La información de un token transaccional generado para el usuario.
        /// </returns>
        public TokenResponseInfo GenerateToken(
            string pinNumber,
            string accountType = null,
            int? amount = null,
            string channelKey = null,
            string metadata = null)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "pinNumber", pinNumber },
                { "accountType", accountType },
                { "amount", amount },
                { "channelKey", channelKey },
                { "metadata", metadata }
            };

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.GenerateToken, body);
            return this.Execute<TokenResponseInfo>(request);
        }

        /// <summary>
        /// Genera un token o clave transaccional para el usuario autenticado.
        /// </summary>
        /// <param name="pinNumber">El pin transaccional del usuario.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="channelKey">El código del canal para el que se emite el token transaccional.</param>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <returns>
        /// La información de un token transaccional generado para el usuario.
        /// </returns>
        public async Task<TokenResponseInfo> GenerateTokenAsync(
            string pinNumber,
            string accountType = null,
            int? amount = null,
            string channelKey = null,
            string metadata = null)
        {
            return await Task.Run(() => this.GenerateToken(pinNumber, accountType, amount, channelKey, metadata));
        }

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

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.TokenChannels);
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
        /// Genera y envía un token o clave transaccional al usuario autenticado.
        /// </summary>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>
        /// Diccionario de datos dinámicos generados por la respuesta.
        /// </returns>
        public DeliveredTokenCustomData SendToken(
            string metadata = null,
            string accountType = null,
            int? amount = null,
            Dictionary<string, string> tags = null)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "metadata", metadata },
                { "accountType", accountType },
                { "amount", amount },
                { "tags", tags }
            };

            IRestRequest request = new AspenRequest(Scope.Delegated, EndpointMapping.SendToken, body);
            return this.Execute<DeliveredTokenCustomData>(request);
        }

        /// <summary>
        /// Genera y envía un token o clave transaccional al usuario autenticado.
        /// </summary>
        /// <param name="metadata">Los metadatos personalizados para asociar al proceso de generación del token transaccional.</param>
        /// <param name="accountType">El identificador del tipo de cuenta de donde se tomarán los fondos de la transacción.</param>
        /// <param name="amount">El valor de la transacción para la que se utilizará el token.</param>
        /// <param name="tags">Colección de claves y valores con información asociada para la generación del token transaccional.</param>
        /// <returns>
        /// Diccionario de datos dinámicos generados por la respuesta.
        /// </returns>
        public async Task<DeliveredTokenCustomData> SendTokenAsync(
            string metadata = null,
            string accountType = null,
            int? amount = null,
            Dictionary<string, string> tags = null)
        {
            return await Task.Run(() => this.SendToken(metadata, accountType, amount, tags));
        }
    }
}