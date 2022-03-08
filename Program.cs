using System;
using System.IO;
using System.Linq;
using System.Net;

namespace imagesDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var errorFilePath = Path.Combine(currentDirectory, "errors.txt");
            if (!File.Exists(errorFilePath))
                File.Create(errorFilePath);

            var downloadFolder = "descargas";
            var downloadDirectory = Path.Combine(currentDirectory, downloadFolder);
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            var linksFileName = Path.Combine(currentDirectory, "links.txt");
            if (!File.Exists(linksFileName))
            {
                using StreamWriter writer = new StreamWriter(errorFilePath, true);
                writer.WriteLine("Mensaje: No existe el archivo con los links. Debe llamarse links.txt");
                return;
            }

            Console.WriteLine($"Hola! Estoy descargando imágenes...");
            if (Directory.EnumerateFileSystemEntries(downloadDirectory).Any())
            {
                Console.WriteLine($"Ojo que la carpeta {downloadFolder} no está vacía.");
            }

            var errors = false;
            foreach (var line in File.ReadLines(linksFileName))
            {
                try
                {
                    var uri = new Uri(line);
                    var outputFileName = Path.GetFileName(uri.LocalPath);
                    var outputFilePath = Path.Combine(downloadDirectory, outputFileName);

                    var i = 0;
                    while (File.Exists(outputFilePath))
                    {
                        outputFilePath = Path.Combine(downloadDirectory,
                            $"{Path.GetFileNameWithoutExtension(outputFileName)} ({++i}){Path.GetExtension(outputFileName)}");
                    }

                    using var client = new WebClient();
                    client.DownloadFile(uri, outputFilePath);
                }
                catch (Exception ex)
                {
                    errors = true;
                    using var writer = new StreamWriter(errorFilePath, true);
                    writer.WriteLine($"Mensaje: {ex.Message}");
                }
            }

            Console.WriteLine($"Listo! Terminé {(errors ? "con errores. Vean el archivo error.txt" : "sin errores!")}");
            Console.WriteLine($"Las imágenes se encuentran en: {downloadDirectory}");
            Console.ReadKey();
        }
    }
}
