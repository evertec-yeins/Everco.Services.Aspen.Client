// -----------------------------------------------------------------------
// <copyright file="IAutonomousApp.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Modules.Autonomous;

    /// <summary>
    /// Define el comportamiento de una aplicación con alcance de autónoma.
    /// </summary>
    public interface IAutonomousApp : IRouting<IAutonomousApp>, IAppIdentity<IAutonomousApp>, ISession<IAutonomousApp>
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        ISettingsModule Settings { get; }

        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        IUtilsModule Utils { get; }

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación, si no se encuentra uno en la cache.
        /// </summary>
        /// <returns>Instancia de <see cref="ISession{TFluent}"/> que permite el acceso a las operaciones del servicio.</returns>
        ISession<IAutonomousApp> Authenticate();

        /// <summary>
        /// Envía al servicio la solicitud de generación de un token de autenticación omitiendo cualquier valor en la cache.
        /// </summary>
        /// <returns>Instancia de <see cref="ISession{TFluent}"/> que permite el acceso a las operaciones del servicio.</returns>
        ISession<IAutonomousApp> AuthenticateNoCache();
    }
}