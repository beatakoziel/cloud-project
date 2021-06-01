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

namespace FolderWatcherService
{
    public class Program
    {
        static TcpListener tcpListener;
        static Socket socket;
        static Thread readThread;
        static Thread mainThread;
        static public HttpClient httpClient;
        static bool isWorking = true;
        public static void Main(string[] args)
        {
            
            mainThread = new Thread(new ThreadStart(() =>
            {
                CreateHostBuilder(args).Build().Run();
            }))
            {
                Name = " main thread ",
                IsBackground = false
            };
            mainThread.Start();
            StartListeningForSignals();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

        static void StartListeningForSignals()
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8090);
            tcpListener.Start();
            socket = tcpListener.AcceptSocket();

            readThread = new Thread(new ThreadStart(ReceiveMessage))
            {
                Name = "Thread listening for signals about new file to download",
                IsBackground = false
            };
            readThread.Start();
        }
        static async void ReceiveMessage()
        {
            try
            {
                while (isWorking)
                {
                    byte[] receivedBytes = new byte[100];
                    _ = socket.Receive(receivedBytes, receivedBytes.Length, 0);
                    string receivedFileId = Encoding.ASCII.GetString(receivedBytes);
                    receivedFileId = receivedFileId.Substring(0, 24);
                    await GetFileFromServer(receivedFileId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static async Task GetFileFromServer(string receivedFileId)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:53033/files/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            FileIdParameterVM fileIdParameterVM = new FileIdParameterVM
            {
                Id = receivedFileId
            };

            FileParameterVM fileResponse = new FileParameterVM();

            var response = await httpClient.GetAsync("service/getFileToService/" + receivedFileId);

            if(response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                fileResponse = JsonConvert.DeserializeObject<FileParameterVM>(jsonString);
            }

            AddFileToDirectory(fileResponse);

            //fileParameter.Name = response.GetType().GetProperties().First(x => x.Name.Equals("Name")).GetValue(response, null).ToString();
            //fileParameter.Type = response.GetType().GetProperties().First(x => x.Name.Equals("Type")).GetValue(response, null).ToString();
            //fileParameter.Directory = response.GetType().GetProperties().First(x => x.Name.Equals("Directory")).GetValue(response, null).ToString();
            //fileParameter.Source = (byte[])response.GetType().GetProperties().First(x => x.Name.Equals("Source")).GetValue(response, null);


        }
        static void AddFileToDirectory(FileParameterVM file)
        {
            string path;
            if(file.Directory.Equals(""))
            {
                if(!Directory.Exists(@"C:\CloudProject\" + file.Directory))
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
