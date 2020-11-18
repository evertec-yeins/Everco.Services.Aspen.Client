// -----------------------------------------------------------------------
// <copyright file="SetUpTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-20 11:18 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using Assets;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las funciones de inicialización de las pruebas implementadas en el espacio de nombres.
    /// </summary>
    [SetUpFixture]
    public class SetUpTests
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string TestingEnvName = "ASPEN:TESTING_EXECUTING";

        /// <summary>
        /// Proporciona un conjunto de instrucciones que se ejecutarán después de finalizar todas las pruebas implementadas.
        /// </summary>
        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            ServicesHelper.Instance.Dispose();
        }

        /// <summary>
        /// Proporciona un conjunto de instrucciones que se ejecutarán por una única vez, antes de iniciar todas las pruebas implementadas.
        /// </summary>
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.SetEnvironmentVariable(TestingEnvName, "true", EnvironmentVariableTarget.Process);
            ServicesHelper.Instance.StartAllServices();
        }
    }
}
