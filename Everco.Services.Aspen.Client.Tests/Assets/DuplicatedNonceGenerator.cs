// -----------------------------------------------------------------------
// <copyright file="DuplicatedNonceGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-25 07:12 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using Providers;

    /// <summary>
    /// Implementa un generador de nonces duplicados a partir de la estructura <see cref="Guid"/>.
    /// </summary>
    public class DuplicatedNonceGenerator : INonceGenerator
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly string nonce = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DuplicatedNonceGenerator" />.
        /// </summary>
        public DuplicatedNonceGenerator() => this.nonce = Guid.NewGuid().ToString("D");

        /// <summary>
        /// Obtiene el nombre con el que se agrega esta información a la solicitud.
        /// </summary>
        public string Name => "Nonce";

        /// <summary>
        /// Obtiene un número o cadena aleatoria para un único uso.
        /// </summary>
        /// <returns>
        /// Una cadena de texto repetida.
        /// </returns>
        public string GetNonce() => this.nonce;
    }
}