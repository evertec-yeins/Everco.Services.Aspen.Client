// -----------------------------------------------------------------------
// <copyright file="TestingEndpointProvider.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-12-10 02:34 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Identity;

    public class TestingEndpointProvider
    {
        public static IEndpointProvider Default => new EnvironmentEndpoint();
    }
}