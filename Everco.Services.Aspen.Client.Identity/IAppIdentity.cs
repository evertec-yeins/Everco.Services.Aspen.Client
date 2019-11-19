// -----------------------------------------------------------------------
// <copyright file="IAppIdentity.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-19 10:44 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define la información que se utiliza para autenticar la solicitud en el servicio Aspen.
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