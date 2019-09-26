// -----------------------------------------------------------------------
// <copyright file="IAppIdentity.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Auth
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