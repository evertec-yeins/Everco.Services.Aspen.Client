// -----------------------------------------------------------------------
// <copyright file="IDelegatedApp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Identity;
    using Modules.Delegated;

    /// <summary>
    /// Define el comportamiento de una aplicación con alcance de delegada.
    /// </summary>
    public interface IDelegatedApp : IRouting<IDelegatedApp>, IAppIdentity<IDelegatedApp>, ISession<IDelegatedApp>
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con las consultas del sistema Aspen.
        /// </summary>
        IInquiriesModule Inquiries { get; }

        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones de administración varias.
        /// </summary>
        IManagementModule Management { get; }

        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        ISettingsModule Settings { get; }

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="docNumber">El número de documento del usuario que firma la solicitud de autenticación.</param>
        /// <param name="password">La clave de acceso del usuario que firma la solicitud de autenticación.</param>
        /// <param name="deviceInfo">La información del dispositivo desde donde se intenta autenticar el usuario.</param>
        /// <returns>Instancia de <see cref="ISession{TFluent}"/> que permite el acceso a las operaciones del servicio.</returns>
        ISession<IDelegatedApp> Authenticate(
            string docType,
            string docNumber,
            string password,
            IDeviceInfo deviceInfo = null);

        /// <summary>
        /// Envía al servicio de Aspen, una solicitud de generación de un token de autenticación firmada con las credenciales de un usuario.
        /// </summary>
        /// <param name="userIdentity">La información de usuario que firma la solicitud de autenticación.</param>
        /// <returns>Instancia de <see cref="ISession{TFluent}"/> que permite el acceso a las operaciones del servicio.</returns>
        ISession<IDelegatedApp> Authenticate(IUserIdentity userIdentity);
    }
}