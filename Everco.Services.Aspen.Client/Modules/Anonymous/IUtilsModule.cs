// -----------------------------------------------------------------------
// <copyright file="IUtilsModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-10-21 11:33 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Anonymous
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
        /// Registra la información de las excepciones que se produzcan por cierres inesperados (AppCrash) de la aplicación.
        /// </summary>
        /// <param name="apiKey">El identificador de la aplicación que generó el error.</param>
        /// <param name="username">El identificador del último usuario que uso la aplicación antes de generarse el error.</param>
        /// <param name="errorReport">La información del reporte de error generado en la aplicación.</param>
        void SaveAppCrash(string apiKey, string username, string errorReport);
    }
}