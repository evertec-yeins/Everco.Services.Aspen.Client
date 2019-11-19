// -----------------------------------------------------------------------
// <copyright file="IUtilsModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-10-21 11:33 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las operaciones para utilidades varias soportadas para una aplicación con alcance de autónoma.
    /// </summary>
    public interface IUtilsModule
    {
        /// <summary>
        /// Obtiene la lista de tipos de documento predeterminados soportados por el servicio.
        /// </summary>
        /// <returns>Lista de tipos de documento predeterminados.</returns>
        IList<DocTypeInfo> GetDefaultDocTypes();

        /// <summary>
        /// Cifra una cadena de texto usando el algoritmo de cifrado del servicio.
        /// </summary>
        /// <param name="value">La cadena de texto en claro a cifrar.</param>
        /// <returns>Cadena cifrada.</returns>
        string Encrypt(string value);
    }
}