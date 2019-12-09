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
    /// Define el comportamiento de una aplicación con alcance de autónoma.
    /// </summary>
    public interface IAnonymous : IRouting, ISession
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a operaciones para utilidades varias.
        /// </summary>
        IUtilsModule Utils { get; }
    }
}