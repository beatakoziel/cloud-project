using FolderWatcherService.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FolderWatcherService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public FileSystemWatcher watcher;
        static public HttpClient httpClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        public static byte[] GetFileContent(string fileName)
        {
            string path = "C:\\CloudProject\\" + fileName;
            byte[] bytes;

            using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

            }

            return bytes;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // watcher
            watcher = new FileSystemWatcher(@"C:\CloudProject");
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            //watcher.Changed += ;
            //watcher.Renamed += ;

            watcher.Created += CreatedWatcher;
            watcher.Deleted += DeletedWatcher;

            // api
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:53033/files/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return base.StartAsync(cancellationToken);
        }
        static async void CreatedWatcher(object sender, FileSystemEventArgs e)
        {
            // file system watcher doesn't see diffrences between files and diectories
            // if e.name equals 'Nowy folder' it means that new directory has been created so it won't be handled in our implementation
            if (e.Name.Equals("Nowy folder"))
                return;

            byte[] fileSource = GetFileContent(e.Name);
            string fileType;
            new FileExtensionContentTypeProvider().TryGetContentType(e.Name, out fileType);

            // reflection to get prop from 'sender' argument
            string path = sender.GetType().GetProperties().First(x => x.Name.Equals("Path")).GetValue(sender, null).ToString();

            FileParameterVM file = new FileParameterVM
            {
                Name = e.Name,
                Source = fileSource,
                Type = fileType,
                Directory = path
            };
            var content = new StringContent(JsonSerializer.Serialize(file).ToString(), Encoding.UTF8, "application/json");
            _ = await httpClient.PostAsync("service/addFile", content);
        }
        static async void DeletedWatcher(object sender, FileSystemEventArgs e)
        {
            // file system watcher doesn't see diffrences between files and diectories
            // if e.name equals 'Nowy folder' it means that directory has been deleted so it won't be handled in our implementation
            if (e.Name.Equals("Nowy folder"))
                return;

            string path = sender.GetType().GetProperties().First(x => x.Name.Equals("Path")).GetValue(sender, null).ToString();

            var content = new StringContent(JsonSerializer.Serialize(new FileDeleteParameterVM() { Name = e.Name, Directory = path }).ToString(), Encoding.UTF8, "application/json"); ;

            _ = await httpClient.PostAsync("service/deleteFile", content);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
