﻿// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Net;
    using Auth;
    using Internals;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : AppBase<IAutonomousApp>, IAutonomousApp
    {
        /// <summary>
        /// Previene la creación de una instancia de la clase <see cref="AutonomousApp"/>
        /// </summary>
        private AutonomousApp()
        {
        }

        /// <summary>
        /// Obtiene una instancia que permite conectar con el servico Aspen.
        /// </summary>
        /// <returns>Instancia de <see cref="IRouting{TFluent}"/> que permite establecer la información de conexión.</returns>
        public static IRouting<IAutonomousApp> Initialize()
        {
            return new AutonomousApp();
        }

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación.
        /// </summary>
        /// <param name="useCache">Cuando es <see langword="true" /> se utiliza el último token de autenticación generado en la sesión.</param>
        /// <returns>
        /// Instancia de <see cref="ISession{TFluent}" /> que permite el acceso a las operaciones del servicio.
        /// </returns>
        public ISession<IAutonomousApp> Authenticate(bool useCache = true)
        {
            if (useCache)
            {
                this.AuthToken = CacheStore.GetCurrentToken();
                if (this.AuthToken != null)
                {
                    return this;
                }
            }

            this.InitializeClient();
            IRestRequest request = new AspenRequest($"{Routes.AutonomousRoot}{Routes.Auth.Signin}", Method.POST);
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSigninPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Uri => {this.RestClient.BaseUrl}{request.Resource}");
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"Method => {request.Method.ToString()}");
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            this.AuthToken = JsonConvert.DeserializeObject<AuthToken>(this.DecodeJwtResponse(response.Content));
            CacheStore.SetCurrentToken(this.AuthToken);
            return this;
        }

        /// <summary>
        /// Envía la solicitud al servicio Aspen.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se convierte la respuesta del servicio Aspen.</typeparam>
        /// <param name="request">Información de la solicitud.</param>
        /// <returns>Instancia de <typeparamref name="TResponse"/> con la información de respuesta del servicio Aspen.</returns>
        /// <exception cref="AspenException">Se presentó un error al procesar la solicitud. La excepción contiene los detalles del error.</exception>
        private TResponse Execute<TResponse>(IRestRequest request) where TResponse : class, new()
        {
            ServiceLocator.Instance.HeadersManager.AddApiKeyHeader(request, this.AppIdentity.ApiKey);
            ServiceLocator.Instance.HeadersManager.AddSignedPayloadHeader(request, this.JwtEncoder, this.AppIdentity.ApiSecret, this.AuthToken.Token);
            IRestResponse response = this.RestClient.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new AspenException(response);
            }

            return response.StatusCode == HttpStatusCode.NoContent
                ? default
                : JsonConvert.DeserializeObject<TResponse>(response.Content);
        }
    }
}