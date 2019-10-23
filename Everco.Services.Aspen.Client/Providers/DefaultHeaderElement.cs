// -----------------------------------------------------------------------
// <copyright file="DefaultHeaderElement.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-19 07:44 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Implementa los nombres que se utilizan en las cabeceras personalizadas de las solicitudes al servicio Aspen.
    /// </summary>
    public class DefaultHeaderElement : IHeaderElement
    {
        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la carga útil (payload)
        /// </summary>
        public string PayloadHeaderName => "X-PRO-Auth-Payload";

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía el ApiKey de la aplicación.
        /// </summary>
        public string ApiKeyHeaderName => "X-PRO-Auth-App";

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la versión del API solicitada.
        /// </summary>
        public string ApiVersionHeaderName => "X-PRO-Api-Version";

        /// <summary>
        /// Obtiene el nombre de la cabecera personalizada en donde se envía la información del dispositivo que origina la solicitud al servicio.
        /// </summary>
        public string DeviceInfoHeaderName => "X-PRO-Request-DeviceInfo";
    }
}