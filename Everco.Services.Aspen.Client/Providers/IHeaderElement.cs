// -----------------------------------------------------------------------
// <copyright file="IHeaderElement.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-19 07:40 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Define los nombres que se utilizan en las cabeceras personalizadas de las solicitudes al servicio Aspen.
    /// </summary>
    public interface IHeaderElement
    {
        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la carga útil (payload)
        /// </summary>
        string PayloadHeaderName { get; }

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía el ApiKey de la aplicación.
        /// </summary>
        string ApiKeyHeaderName { get; }

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la versión del API solicitada.
        /// </summary>
        string ApiVersionHeaderName { get; }

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la información del dispositivo que origina la solicitud al servicio.
        /// </summary>
        string DeviceInfoHeaderName { get; }
    }
}