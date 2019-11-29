﻿// -----------------------------------------------------------------------
// <copyright file="CacheStore.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-01-03 04:44 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using System;
    using System.Collections.Generic;
    using Auth;
    using Identity;

    /// <summary>
    /// Permite almacenar el último token de autenticación generado.
    /// </summary>
    internal static class CacheStore
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string DeviceCacheKey = "CURRENT_DEVICE_INFO";

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string TokenCacheKey = "CURRENT_AUTH_TOKEN";

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static readonly List<string> keys;

        /// <summary>
        /// Inicializa los miembros estáticos de la clase <see cref="CacheStore"/>
        /// </summary>
        static CacheStore()
        {
            keys = new List<string>();
        }

        /// <summary>
        /// Obtiene el último token de autenticación generado o  <see langword="null" /> si no se ha obtenido ninguno.
        /// </summary>
        /// <param name="apiKey">ApiKey para el que se obtiene el token de autemticación de la cache.</param>
        /// <returns>Instancia que implementa <see cref="IAuthToken" /> con el valor del último token generado.</returns>
        internal static IAuthToken GetCurrentToken(string apiKey)
        {
            string cacheKey = $"{TokenCacheKey}-{apiKey}";
            keys.Add(cacheKey);
            return AppDomain.CurrentDomain.GetData(cacheKey) as IAuthToken;
        }

        /// <summary>
        /// Obtiene la ifnormación del dispositivo solicitante o  <see langword="null" /> si no se ha guardado información.
        /// </summary>
        /// <returns>Instancia que implementa <see cref="IDeviceInfo"/> con al información del dispositivo.</returns>
        internal static IDeviceInfo GetDeviceInfo()
        {
            return AppDomain.CurrentDomain.GetData(DeviceCacheKey) as IDeviceInfo;
        }

        /// <summary>
        /// Reestablece a <see langword="null" /> todos los valores en la cache.
        /// </summary>
        internal static void Reset()
        {
            foreach (string key in keys)
            {
                AppDomain.CurrentDomain.SetData(key, null);
            }

            keys.Clear();
            AppDomain.CurrentDomain.SetData(DeviceCacheKey, null);
        }

        /// <summary>
        /// Guarda el último token de autenticación generado.
        /// </summary>
        /// <param name="apiKey">ApiKey con el que se asocia el token de autemticación en la cache.</param>
        /// <param name="authToken">Instancia del token que se debe guardar.</param>
        internal static void SetCurrentToken(string apiKey, IAuthToken authToken)
        {
            string cacheKey = $"{TokenCacheKey}-{apiKey}";
            keys.Add(cacheKey);
            AppDomain.CurrentDomain.SetData(cacheKey, authToken);
        }

        /// <summary>
        /// Guarda la ifnormación del dispositivo actual.
        /// </summary>
        /// <param name="deviceInfo">Instancia del dispositivo que se debe guardar.</param>
        internal static void SetDeviceInfo(IDeviceInfo deviceInfo)
        {
            AppDomain.CurrentDomain.SetData(DeviceCacheKey, deviceInfo);
        }
    }
}