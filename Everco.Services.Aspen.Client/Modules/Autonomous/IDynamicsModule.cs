// -----------------------------------------------------------------------
// <copyright file="IDynamicsModule.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-02 03:06 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;

    /// <summary>
    /// Define operaciones dinámicas para el servicio Aspen.
    /// </summary>
    public interface IDynamicsModule
    {
        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        TResponse Delete<TRequest, TResponse>(string resource, TRequest body) where
            TRequest : class
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        TResponse Delete<TResponse>(string resource)
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        void Delete(string resource);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Delete(string resource, IDictionary<string, object> body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Delete(string resource, params KeyValuePair<string, object>[] body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        TResponse Get<TRequest, TResponse>(string resource, TRequest body) where
            TRequest : class
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        TResponse Get<TResponse>(string resource)
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        void Get(string resource);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Get(string resource, IDictionary<string, object> body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        TResponse Post<TRequest, TResponse>(string resource, TRequest body) where 
            TRequest : class
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        TResponse Post<TResponse>(string resource)
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        void Post(string resource);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Post(string resource, IDictionary<string, object> body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Post(string resource, params KeyValuePair<string, object>[] body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        TResponse Put<TRequest, TResponse>(string resource, TRequest body) where
            TRequest : class
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        TResponse Put<TResponse>(string resource)
            where TResponse : class, new();

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        void Put(string resource);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Put(string resource, IDictionary<string, object> body);

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        void Put(string resource, params KeyValuePair<string, object>[] body);
    }
}