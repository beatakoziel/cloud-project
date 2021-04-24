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
        public static string GetFileContent(string fileName)
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

            return Convert.ToBase64String(bytes);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // watcher
            watcher = new FileSystemWatcher(@"C:\CloudProject");
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;

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
            string fileSource = GetFileContent(e.Name);
            string fileType;
            new FileExtensionContentTypeProvider().TryGetContentType(e.Name, out fileType);
            FileVM file = new FileVM
            {
                Name = e.Name,
                Source = fileSource,
                Type = fileType
            };
            var content = new StringContent(JsonSerializer.Serialize(file).ToString(), Encoding.UTF8, "application/json");
            _ = await httpClient.PostAsync("", content);
        }
        static void DeletedWatcher(object sender, FileSystemEventArgs e)
        {
            //TODO
            throw new NotImplementedException();
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
