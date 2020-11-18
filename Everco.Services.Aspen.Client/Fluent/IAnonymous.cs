// -----------------------------------------------------------------------
// <copyright file="IAnonymous.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-12-09 15:35 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using Everco.Services.Aspen.Client.Modules.Anonymous;

    /// <summary>
    /// Define el comportamiento de las operaciones disponibles en el servicio Aspen que no requieren firma.
    /// </summary>
    public interface IAnonymous : IRouting, ISession
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización de la aplicación en el sistema Aspen.
        /// </summary>
        ISettingsModule Settings { get; }

        /// <summary>
        /// Obtiene un objeto que permite acceder a las operaciones disponibles para utilidades varias en el sistema de Aspen.
        /// </summary>
        IUtilsModule Utils { get; }
    }
}