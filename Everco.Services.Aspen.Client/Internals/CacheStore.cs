// -----------------------------------------------------------------------
// <copyright file="CacheStore.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-01-03 04:44 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
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
        /// Agrega una entrada al caché.
        /// </summary>
        /// <typeparam name="TEntry">El tipo de la entrada a guardar.</typeparam>
        /// <param name="key">La clave que identifica la entrada a guardar.</param>
        /// <param name="item">El valor o información de la enterada que se desea guardar.</param>
        internal static void Add<TEntry>(string key, TEntry item)
        {
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
            ServiceLocator.Instance.LoggingProvider.WriteDebug($"CacheStore => Getting entry: '{key}' from cache.");
            return cache.Get<TEntry>(key);
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
    }
}