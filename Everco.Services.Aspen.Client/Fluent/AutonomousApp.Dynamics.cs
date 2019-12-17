// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Dynamics.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-02 03:25 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Internals;
    using Modules.Autonomous;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public partial class AutonomousApp : IDynamicsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a los endpoints dinámicos del servicio.
        /// </summary>
        public IDynamicsModule Dynamics => this;

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        public TResponse Delete<TRequest, TResponse>(string resource, TRequest body)
            where TRequest : class
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.DELETE, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        public TResponse Delete<TResponse>(string resource)
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.DELETE, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        public void Delete(string resource)
        {
            this.Submit<ExpandoObject>(resource, Method.DELETE, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Delete(string resource, IDictionary<string, object> body)
        {
            this.Submit<ExpandoObject>(resource, Method.DELETE, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.DELETE" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Delete(string resource, params KeyValuePair<string, object>[] body)
        {
            IEnumerable<KeyValuePair<string, object>> kvp = body ?? Enumerable.Empty<KeyValuePair<string, object>>();
            this.Submit<ExpandoObject>(resource, Method.DELETE, kvp.ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        public TResponse Get<TRequest, TResponse>(string resource, TRequest body)
            where TRequest : class
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.GET, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        public TResponse Get<TResponse>(string resource) where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.GET, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        public void Get(string resource)
        {
            this.Submit<ExpandoObject>(resource, Method.GET, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.GET" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Get(string resource, IDictionary<string, object> body)
        {
            this.Submit<ExpandoObject>(resource, Method.GET, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        public TResponse Post<TRequest, TResponse>(string resource, TRequest body)
            where TRequest : class
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.POST, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        public TResponse Post<TResponse>(string resource)
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.POST, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        public void Post(string resource)
        {
            this.Submit<ExpandoObject>(resource, Method.POST, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Post(string resource, IDictionary<string, object> body)
        {
            this.Submit<ExpandoObject>(resource, Method.POST, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Post(string resource, params KeyValuePair<string, object>[] body)
        {
            IEnumerable<KeyValuePair<string, object>> kvp = body ?? Enumerable.Empty<KeyValuePair<string, object>>();
            this.Submit<ExpandoObject>(resource, Method.POST, kvp.ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <typeparam name="TRequest">Tipo del objeto que se envía con la solicitud.</typeparam>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Instancia de un objeto que se envia en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que representa la respuesta generada por el servicio.</returns>
        public TResponse Put<TRequest, TResponse>(string resource, TRequest body)
            where TRequest : class
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.PUT, body);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se castea la respuesta del servicio.</typeparam>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <returns>Instancia que representa la respuesta generada por el servicio.</returns>
        public TResponse Put<TResponse>(string resource)
            where TResponse : class, new()
        {
            return this.Submit<TResponse>(resource, Method.PUT, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <exception cref="AspenException"> el servicio generó una respuesta diferente a OK (200).</exception>
        public void Put(string resource)
        {
            this.Submit<ExpandoObject>(resource, Method.PUT, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.PUT" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Put(string resource, IDictionary<string, object> body)
        {
            this.Submit<ExpandoObject>(resource, Method.PUT, null);
        }

        /// <summary>
        /// Envia una solicitud <see cref="RestSharp.Method.POST" /> al servicio.
        /// </summary>
        /// <param name="resource">URL de la operación expuesta por el servicio.</param>
        /// <param name="body">Valores que se envian en el cuerpo de la solicitud.</param>
        public void Put(string resource, params KeyValuePair<string, object>[] body)
        {
            IEnumerable<KeyValuePair<string, object>> kvp = body ?? Enumerable.Empty<KeyValuePair<string, object>>();
            this.Submit<ExpandoObject>(resource, Method.PUT, kvp.ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        /// <summary>
        /// Envía la solicitud al servicio.
        /// </summary>
        /// <typeparam name="TResponse">Tipo del objeto al que se convierte la respuesta del servicio.</typeparam>
        /// <param name="resource">Url del recurso solicitado.</param>
        /// <param name="method">Método o verbo HTTP para invocar el recurso.</param>
        /// <param name="body">Datos que se envian en el cuerpo de la solicitud.</param>
        /// <returns>Instancia de TResponse que represeta los datos de respuesta del servicio.</returns>
        private TResponse Submit<TResponse>(string resource, Method method, object body)
            where TResponse : class, new()
        {
            string url = $"/app/ext/{resource.TrimStart('/').TrimEnd('/')}";
            AspenRequest request = new AspenRequest(url, method);

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            return this.Execute<TResponse>(request);
        }
    }
}