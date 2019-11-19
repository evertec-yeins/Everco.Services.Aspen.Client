// -----------------------------------------------------------------------
// <copyright file="SingleUseNonceGenerator.cs" company="Evertec Colombia">
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
    /// Implementa un generador de nonces de un solo uso a partir de la estructura <see cref="Guid"/>.
    /// </summary>
    public class SingleUseNonceGenerator : INonceGenerator
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly string nonce = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SingleUseNonceGenerator" />.
        /// </summary>
        public SingleUseNonceGenerator() => this.nonce = Guid.NewGuid().ToString("D");

        /// <summary>
        /// Obtiene un número o cadena aleatoria para un único uso.
        /// </summary>
        /// <returns>
        /// Una cadena de texto repetida.
        /// </returns>
        public string GetNonce() => this.nonce;
    }
}