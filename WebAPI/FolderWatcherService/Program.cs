using FolderWatcherService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FolderWatcherService
{
    [Obsolete]
    public class Program
    {
        private static HttpClient _httpClient;
        
        public static void Main(string[] args)
        {
            InitHttpClient();
               
            CreateHostBuilder(args).Build().Run();
        }
        
        private static void InitHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:53033/files/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
        public static async Task GetFileFromServer(string receivedFileId)
        {
            FileIdParameterVM fileIdParameterVM = new FileIdParameterVM
            {
                Id = receivedFileId
            };

            FileParameterVM fileResponse = new FileParameterVM();

            var response = await _httpClient.GetAsync("service/getFileToService/" + receivedFileId);

            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                fileResponse = JsonConvert.DeserializeObject<FileParameterVM>(jsonString);
            }

            AddFileToDirectory(fileResponse);
        }
        static void AddFileToDirectory(FileParameterVM file)
        {
            string path;
            if (!file.Directory.Equals(""))
            {
                if (!Directory.Exists(@"C:\CloudProject\" + file.Directory))
                {
                    Directory.CreateDirectory(@"C:\CloudProject\" + file.Directory);
                }
                path = @"C:\CloudProject\" + file.Directory + @"\" + file.Name;
            }
            else
            {
                path = @"C:\CloudProject\" + file.Name;
            }

            BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(path));

            binaryWriter.Write(file.Source);
            binaryWriter.Flush();
            binaryWriter.Close();
        }
    }
}
