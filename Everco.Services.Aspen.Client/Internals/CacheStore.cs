// -----------------------------------------------------------------------
// <copyright file="CacheStore.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-01-03 04:44 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using Auth;
    using LazyCache;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Implementa las funcionalidades que permiten almacenar información en caché.
    /// </summary>
    internal static class CacheStore
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static readonly IAppCache cache;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static readonly MemoryCacheEntryOptions cacheOptions;

        /// <summary>
        /// Inicializa los miembros estáticos de la clase <see cref="CacheStore"/>
        /// </summary>
        static CacheStore()
        {
            cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };

            cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                ServiceLocator.Instance.LoggingProvider.WriteDebug($"CacheStore => Entry: '{key}' is evicted from cache. Reason: {reason}");
            });

            cache = new CachingService();
        }

        /// <summary>
        /// Obtiene o establece la política para el tratamiento de la información almacenada por caché.
        /// </summary>
        internal static CachePolicy Policy { get; set; }

        /// <summary>
        /// Agrega una entrada al caché.
        /// </summary>
        /// <typeparam name="TEntry">El tipo de la entrada a guardar.</typeparam>
        /// <param name="key">La clave que identifica la entrada a guardar.</param>
        /// <param name="item">El valor o información de la enterada que se desea guardar.</param>
        internal static void Add<TEntry>(string key, TEntry item)
        {
            if (Policy == CachePolicy.BypassCache)
            {
                return;
            }

            cache.Add(key, item, cacheOptions);
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"CacheStore => Entry: '{key}' saved to cache.");
        }

        /// <summary>
        /// Obtiene una entrada almacenada en el caché.
        /// </summary>
        /// <typeparam name="TEntry">El tipo de la entrada.</typeparam>
        /// <param name="key">La clave con la que se guardó la entrada en el caché.</param>
        /// <returns>Una instancia de <typeparamref name="TEntry"/> que representa al valor o información recuperado del caché.</returns>
        internal static TEntry Get<TEntry>(string key)
        {
            if (Policy == CachePolicy.BypassCache)
            {
                return default;
            }

            ServiceLocator.Instance.LoggingProvider.WriteDebug($"CacheStore => Getting entry: '{key}' from cache.");
            return cache.Get<TEntry>(key);
        }

        /// <summary>
        /// Obtiene el último token de autenticación generado o <see langword="null" /> si no se ha emitido ninguno.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación para el que se obtiene el token de autenticación del caché.</param>
        /// <returns>Instancia de <see cref="IAuthToken" /> con la información del último token de autenticación generado.</returns>
        internal static IAuthToken GetCurrentAuthToken(string apiKey)
        {
            string cacheKey = $"{CacheKeys.CurrentAuthToken}:{apiKey.ToUpper()}";
            return ServiceLocator.Instance.Runtime.IsTestingExecuting
                ? null
                : Get<AuthToken>(cacheKey);
        }

        /// <summary>
        /// Remueve una entrada almacenada en el caché.
        /// </summary>
        /// <param name="key">La clave con la que se guardó la entrada en el caché.</param>
        internal static void Remove(string key)
        {
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"CacheStore => Removing entry: '{key}' saved on cache.");
            cache.Remove(key);
        }

        /// <summary>
        /// Guarda la información del último token de autenticación emitido.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación con el que se asocia el token de autenticación en el caché.</param>
        /// <param name="authToken">La información del token de autenticación que se desea guardar.</param>
        internal static void SetCurrentAuthToken(string apiKey, IAuthToken authToken)
        {
            string cacheKey = $"{CacheKeys.CurrentAuthToken}:{apiKey.ToUpper()}";
            Add(cacheKey, authToken);
        }
    }
}