// -----------------------------------------------------------------------
// <copyright file="IUtilsModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-10-21 11:33 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Delegated
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Define las operaciones soportadas apara acceder a entidades de información relacionadas con parametrización del sistema.
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

        /// <summary>
        /// Registra la información del crash generado en la aplicación.
        /// </summary>
        /// <param name="errorReport">Información del reporte de error generado en la aplicación.</param>
        /// <param name="userName">Información del último usuario que uso la aplicación antes de generarse el error.</param>
        void SaveAppCrash(string errorReport, string userName);
    }
}