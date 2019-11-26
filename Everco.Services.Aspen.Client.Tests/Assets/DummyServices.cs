// -----------------------------------------------------------------------
// <copyright file="DummyServices.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-20 12:04 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Implementa funcionalidades para administrar servicios ficticios creados a partir de queries de LINQPad.
    /// </summary>
    public class DummyServices : IDisposable
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DummyServices" />.
        /// </summary>
        public DummyServices()
        {
            this.DummyFilesPath = Path.Join(Directory.GetCurrentDirectory(), @"Assets\Dummies");
        }

        /// <summary>
        /// Obtiene la ruta de los archivos ficticios.
        /// </summary>
        public string DummyFilesPath { get; private set; }

        /// <summary>
        /// Realiza tareas asociadas con la liberación o restablecimiento de recursos no administrados.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("lprun");
                foreach (Process process in processes)
                {
                    process.Kill(true);
                    process.Close();
                    process.Dispose();
                }

                Directory.GetFiles(this.DummyFilesPath, "*.flag", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Inicia todos los servicios de prueba disponibles.
        /// </summary>
        /// <returns>La instancia actual de <see cref="DummyServices"/>.</returns>
        public DummyServices StartAllServices()
        {
            foreach (FileInfo dummyFileInfo in this.GetDummyFiles())
            {
                this.StartProcess(dummyFileInfo);
            }

            return this;
        }

        /// <summary>
        /// Inicia el microservicio de Bifrost.
        /// </summary>
        /// <returns>La instancia actual de <see cref="DummyServices"/>.</returns>
        public DummyServices StartBifrostService()
        {
            FileInfo dummyFileInfo = this.GetDummyFileInfo("RabbitMQ.Services.Bifrost.linq");
            this.StartProcess(dummyFileInfo);
            return this;
        }

        /// <summary>
        /// Obtiene la información de un archivo <c>.linq</c> a partir del nombre.
        /// </summary>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <returns>Instancia de <see cref="FileInfo"/> con la información del archivo.</returns>
        private FileInfo GetDummyFileInfo(string fileName) => this.GetDummyFiles().First(file => file.Name == fileName);

        /// <summary>
        /// Obtiene la información de los archivos con extensión <c>.linq</c> disponibles.
        /// </summary>
        /// <returns>Lista de <see cref="FileInfo"/> con la información de los archivos linq de pruebas.</returns>
        private IEnumerable<FileInfo> GetDummyFiles()
        {
            return Directory.GetFiles(this.DummyFilesPath, "*.linq", SearchOption.TopDirectoryOnly)
                .Select(path => new FileInfo(path));
        }

        /// <summary>
        /// Inicia y agrega a la colección actual, un proceso a partir de la información del archivo de servicio ficticio.
        /// </summary>
        /// <param name="dummyFileInfo">La información del archivo ficticio.</param>
        private void StartProcess(FileInfo dummyFileInfo)
        {
            string key = dummyFileInfo.Name;
            ProcessStartInfo startInfo = new ProcessStartInfo("lprun.exe")
                                             {
                                                 WindowStyle = ProcessWindowStyle.Minimized,
                                                 Arguments = dummyFileInfo.FullName
                                             };
            Process.Start(startInfo);

            // Aquí se pretende dar una espera, hasta comprobar que el servicio realmente inició.
            DateTime waitTime = DateTime.Now.AddSeconds(10);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dummyFileInfo.FullName);
            string flagFilePath = Path.Join(dummyFileInfo.Directory?.FullName, $"{fileNameWithoutExtension}.flag");
            
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Console.WriteLine($@"[INFO] - [{DateTime.Now:HH:mm:ss}] Starting Dummy Service => {dummyFileInfo.Name}");
            }
            while (!File.Exists(flagFilePath) && waitTime > DateTime.Now);
            Console.WriteLine($@"[INFO] - [{DateTime.Now:HH:mm:ss}] Started Dummy Service => {dummyFileInfo.Name}");
        }
    }
}
