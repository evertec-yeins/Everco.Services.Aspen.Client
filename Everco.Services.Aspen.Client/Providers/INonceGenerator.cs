// -----------------------------------------------------------------------
// <copyright file="INonceGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Define los métodos para un generador de números o cadenas aleatorios para un único uso.
    /// </summary>
    /// <remarks>El nonce permite garantizar que las comunicaciones antiguas no se puedan reutilizar por ataques de repetición.</remarks>
    public interface INonceGenerator
    {
        /// <summary>
        /// Obtiene un número o cadena aleatoria para un único uso.
        /// </summary>
        /// <returns>Cadena de texto aleatoria para un único uso.</returns>
        string GetNonce();
    }
}