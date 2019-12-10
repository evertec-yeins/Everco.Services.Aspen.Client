// -----------------------------------------------------------------------
// <copyright file="ServicesHelper.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-20 12:04 PM</date>
// -----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Implementa funcionalidades para administrar servicios ficticios creados a partir de queries de LINQPad.
    /// </summary>
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Keep calm and ignore")]
    public class ServicesHelper : IDisposable
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static ServicesHelper instance;

        /// <summary>
        /// Los procesos que fueron iniciados por cada servicio ficticio.
        /// </summary>
        private Dictionary<string, Process> processesStarted = null;

        /// <summary>
        /// Impide que se cree una instancia predeterminada de la clase <see cref="ServicesHelper" />.
        /// </summary>
        private ServicesHelper()
        {
            this.DummyFilesPath = Path.Join(Directory.GetCurrentDirectory(), @"Assets\Dummies");
            this.processesStarted = new Dictionary<string, Process>();
        }

        /// <summary>
        /// The instance.
        /// </summary>
        public static ServicesHelper Instance => instance ?? (instance = new ServicesHelper());

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
                IEnumerable<Process> killProcesses = Process.GetProcessesByName("lprun");
                foreach (Process process in killProcesses)
                {
                    process.Kill(true);
                    process.Close();
                    process.Dispose();
                }

                Directory.GetFiles(this.DummyFilesPath, "*.flag", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
                this.processesStarted?.Clear();
                this.processesStarted = null;
                instance = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($@"[ERROR] Unhandled exception on killing dummy processes: {exception.Message}");
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Inicializa todos los servicios ficticios disponibles.
        /// </summary>
        /// <returns>La instancia actual de <see cref="ServicesHelper"/>.</returns>
        public ServicesHelper StartAllServices()
        {
            foreach (FileInfo dummyFileInfo in this.GetDummyFiles())
            {
                this.StartProcess(dummyFileInfo);
            }

            return this;
        }

        /// <summary>
        /// Inicializa el microservicio de Bancor.
        /// </summary>
        /// <returns>La instancia actual de <see cref="ServicesHelper"/>.</returns>
        public ServicesHelper StartBancorService()
        {
            FileInfo dummyFileInfo = this.GetDummyFileInfo("RabbitMQ.Services.Bancor.linq");
            this.StartProcess(dummyFileInfo);
            return this;
        }

        /// <summary>
        /// Inicializa el microservicio de Bifrost.
        /// </summary>
        /// <returns>La instancia actual de <see cref="ServicesHelper"/>.</returns>
        public ServicesHelper StartBifrostService()
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
            if (this.processesStarted.ContainsKey(key))
            {
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo("lprun.exe")
                                             {
                                                 WindowStyle = ProcessWindowStyle.Minimized,
                                                 Arguments = dummyFileInfo.FullName,
                                                 CreateNoWindow = true
                                             };
            this.processesStarted.Add(key, Process.Start(startInfo));

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
