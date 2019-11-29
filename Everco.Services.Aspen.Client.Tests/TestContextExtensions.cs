// -----------------------------------------------------------------------
// <copyright file="TestContextExtensions.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-26 09:39 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Data;
    using NUnit.Framework;

    /// <summary>
    /// Expone métodos de extensión para los tipos <see cref="TestContext"/>.
    /// </summary>
    public static class TestContextExtensions
    {
        /// <summary>
        /// Retorna una instancia de <see cref="IDatabaseHelper"/> que permite el acceso a la base de datos del servicio.
        /// </summary>
        /// <param name="testContext">Instancia del contexto de pruebas.</param>
        /// <returns>Instancia de <see cref="IDatabaseHelper"/> que permite el acceso a la base de datos del servicio.</returns>
        public static IDatabaseHelper DatabaseHelper(this TestContext testContext)
        {
            return new SqlServerHelper();
        }
    }
}