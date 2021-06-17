using FileWatcher.VMs;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FileWatcher
{
    public class WebSocketHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"Incoming file ID -> {e.Data}");

            Program.GetFileFromServer(e.Data);
        }
    }
    public class SynchronizeHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"Synchronizing fired");

            Program.SynchronizeDirectory();
        }
    }
    class Program
    {
        //public FileSystemWatcher _watcher;
        private static WebSocketServer _webSocket;
        static public HttpClient _httpClient;
        static void Main(string[] args)
        {
            InitWebSocketServer();
            InitHttpClient();

            using var watcher = new FileSystemWatcher(@"C:\CloudProject");
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Created += CreatedWatcher;
            watcher.Deleted += DeletedWatcher;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            _webSocket.Stop();
        }
        private static void InitWebSocketServer()
        {
            _webSocket = new WebSocketServer("ws://127.0.0.1:4649");
            _webSocket.AddWebSocketService<WebSocketHandler>("/FileAdded");
            _webSocket.AddWebSocketService<SynchronizeHandler>("/Synchronize");
            _webSocket.Start();
            Console.WriteLine("Web socket started");
        }
        private static void InitHttpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:53033/files/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.MaxResponseContentBufferSize = 2147483647;
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        static async void CreatedWatcher(object sender, FileSystemEventArgs e)
        {
            // just because, that's why
            Thread.Sleep(3000);
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
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(file).ToString(), Encoding.UTF8, "application/json");
            _ = await _httpClient.PostAsync("service/addFile", content);
        }
        static async void DeletedWatcher(object sender, FileSystemEventArgs e)
        {
            // file system watcher doesn't see diffrences between files and diectories
            // if e.name equals 'Nowy folder' it means that directory has been deleted so it won't be handled in our implementation
            if (e.Name.Equals("Nowy folder"))
                return;

            string path = sender.GetType().GetProperties().First(x => x.Name.Equals("Path")).GetValue(sender, null).ToString();

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new FileDeleteParameterVM() { Name = e.Name, Directory = path }).ToString(), Encoding.UTF8, "application/json"); ;

            _ = await _httpClient.PostAsync("service/deleteFile", content);
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
        public static async Task GetFileFromServer(string receivedFileId)
        {
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
        public static async Task SynchronizeDirectory()
        {
            //await ClearDirectory();
            
            var response = await _httpClient.GetAsync("service/synchronize");
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                HandleSynchronize(jsonString);
            }
        }
        static void HandleSynchronize(string jsonString)
        {
            List<FileParameterVM> files = new List<FileParameterVM>();
            files = JsonConvert.DeserializeObject<List<FileParameterVM>>(jsonString);
            if (files != null)
            {
                foreach (var item in files)
                {
                    AddFileToDirectory(item);
                }
            }
        }
        static async Task ClearDirectory()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\CloudProject");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            } 
        }
    }
}
