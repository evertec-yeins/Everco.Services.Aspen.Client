// -----------------------------------------------------------------------
// <copyright file="GuidNonceGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Implementa un generador de números arbitrarios que solo se puede utilizar una vez a partir de la estructura <see cref="Guid"/>.
    /// </summary>
    /// <remarks>El nonce permite garantizar que las comunicaciones antiguas no se puedan reutilizar por ataques de repetición.</remarks>
    public class GuidNonceGenerator : INonceGenerator
    {
        /// <summary>
        /// Obtiene el nombre con el que se agrega esta información a la solicitud.
        /// </summary>
        public string Name => "Nonce";

        /// <summary>
        /// Obtiene un número o cadena aleatoria para un único uso.
        /// </summary>
        /// <returns>
        /// Cadena de texto aleatoria para un único uso.
        /// </returns>
        public string GetNonce()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}