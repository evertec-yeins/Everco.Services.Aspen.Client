// -----------------------------------------------------------------------
// <copyright file="ISecureStorage.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-26 02:04 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define operaciones para guardar una pareja ApiKey/AppiSecret de forma segura.
    /// </summary>
    public interface ISecureStorage
    {
        /// <summary>
        /// Guarda en un archivo la información.
        /// </summary>
        /// <param name="path">Ruta de acceso del archivo donde se guarda la información.</param>
        void SaveTo(string path);
    }
}