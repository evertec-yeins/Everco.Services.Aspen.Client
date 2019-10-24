// -----------------------------------------------------------------------
// <copyright file="TestDemo.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-10-23 03:50 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestDemo
    {
        [Test]
        public void Demo()
        {
            Assert.That(1+1, Is.EqualTo(2));
        }

        [Test]
        public void Demo2()
        {
            Assert.That(true, Is.True);
        }

    }
}