// -----------------------------------------------------------------------
// <copyright file="IAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 10:44 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define la información de identidad de una aplicación para autenticar las solicitudes al servicio ASPEN.
    /// </summary>
    /// <remarks>Estos valores son proporcionados por Evertec Colombia.</remarks>
    public interface IAppIdentity
    {
        /// <summary>
        /// Obtiene el ApiKey que identifica a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Obtiene el ApiSecret asociado a la aplicación que intenta autenticar la solicitud.
        /// </summary>
        string ApiSecret { get; }
    }
}