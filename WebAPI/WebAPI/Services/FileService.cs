using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebAPI.Repositories;
using WebAPI.ViewModels;
using File = WebAPI.Models.File;

namespace WebAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IDirectoryRepository directoryRepository;

        public FileService(IFileRepository fileRepository, IDirectoryRepository directoryRepository)
        {
            this.fileRepository = fileRepository;
            this.directoryRepository = directoryRepository;
        }

        public List<FileVM> GetAllFiles()
        {
            return fileRepository.GetAllFiles();
        }

        public List<FileVM> GetCurrentFiles(string dirId)
        {
            return fileRepository.GetCurrentFiles(dirId);
        }

        public File GetFileById(string id)
        {
            return fileRepository.GetFileById(id);
        }

        public FileVM GetCurrentFile(string name)
        {
            return fileRepository.GetCurrentFileByName(name);
        }

        public void AddFile(IFormFile data, string dirId)
        {
            string rootPath = @"D:\cloud-project-storage\";

            List<File> previousFiles = fileRepository.GetFiles()
                .FindAll(f => f.Name.Equals(data.FileName) && f.ContentType.Equals(data.ContentType) && f.DirectoryId.Equals(dirId));
            File newFile = new File();
            newFile.Name = data.FileName;
            newFile.ContentType = data.ContentType;
            newFile.CreatedDate = DateTime.Now;
            newFile.DirectoryId = dirId;
            newFile.IsCurrent = true;
            fileRepository.AddFile(newFile);
            string filePath = rootPath + newFile.Id + "." +
                              data.FileName.Substring(data.FileName.IndexOf(".", StringComparison.Ordinal) + 1);
            ;
            newFile.Path = filePath;
            fileRepository.EditFile(newFile);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                data.CopyTo(fileStream);
                fileStream.Flush();
            }

            if (previousFiles.Count > 0)
            {
                File previousFile = previousFiles
                    .OrderBy(f => f.CreatedDate).First();
                newFile.ParentId = previousFile.Id;
                fileRepository.EditFile(newFile);
                foreach (var file in previousFiles)
                {
                    file.IsCurrent = false;
                    file.CurrentId = newFile.Id;
                    fileRepository.EditFile(file);
                }
            }

            SendSignalToService(newFile.Id);
        }


        public void EditFile(File data)
        {
            fileRepository.EditFile(data);
        }

        public void DeleteFile(string fileId)
        {
            File file = GetFileById(fileId);
            List<File> previousFiles = fileRepository.GetFiles()
                .FindAll(f => f.Name.Equals(file.Name) && f.ContentType.Equals(file.ContentType) && f.DirectoryId.Equals(file.DirectoryId));
            foreach (var fileToDelete in previousFiles)
            {
                DeleteFileById(fileToDelete.Id);
            }
        }

        public void DeleteFileById(string fileId)
        {
            File file = GetFileById(fileId);
            if (System.IO.File.Exists(file.Path))
            {
                System.IO.File.Delete(file.Path);
                fileRepository.DeleteFile(fileId);
            }
        }

        public FileSourceVM GetFileSource(string fileId)
        {
            File file = GetFileById(fileId);
            byte[] source = System.IO.File.ReadAllBytes(file.Path);
            return new FileSourceVM
            {
                Name = file.Name,
                ContentType = file.ContentType,
                Source = source
            };
        }

        public void DeleteFileByName(string fileName)
        {
            List<File> files = fileRepository.GetFilesByName(fileName);
            foreach (var file in files)
            {
                DeleteFileById(file.Id);
            }
        }
        public void AddFileFromService(FileParameterVM file)
        {
            string dirName = "";
            if (file.Name.Contains(@"\"))
            {
                int lastSlashPos = file.Name.LastIndexOf(@"\") + 1;
                dirName = file.Name.Substring(0, lastSlashPos);
                dirName = dirName.Replace(@"\", "");
                file.Name = file.Name.Remove(0, lastSlashPos);
            }

            if (file.Directory.Equals("C:\\CloudProject"))
            {
                file.Directory = "";
            }

            List<Models.Directory> directories = directoryRepository.GetAllDirectories();
            string dirId;
            if (file.Directory.Equals(""))
            {
                dirId = "0";
            }
            else
            {
                if (directories.Any(x => x.Name.Equals(dirName)))
                {
                    dirId = directoryRepository.GetDirIdByName(dirName);
                }
                else
                {
                    directoryRepository.AddDirectory("0", dirName);
                    dirId = directoryRepository.GetDirIdByName(dirName);
                }
            }
            AddOrUpdateFile(file, dirId);
        }

        internal void AddOrUpdateFile(FileParameterVM file, string dirId)
        {
            string rootPath = @"D:\cloud-project-storage\";

            // create new file instance
            File newFile = new File
            {
                Name = file.Name,
                ContentType = file.Type,
                IsCurrent = true,
                CreatedDate = DateTime.Now,
                DirectoryId = dirId
            };
            fileRepository.AddFile(newFile);

            // generate name to save on "FTP"
            string filePath = rootPath + newFile.Id + "." +
                              file.Name.Substring(file.Name.IndexOf(".", StringComparison.Ordinal) + 1);
            ;
            newFile.Path = filePath;
            fileRepository.EditFile(newFile);

            // save physically file on "FTP"
            BinaryWriter binaryWriter = null;

            binaryWriter = new BinaryWriter(System.IO.File.OpenWrite(newFile.Path));
            binaryWriter.Write(file.Source);
            binaryWriter.Flush();
            binaryWriter.Close();


            // handle previous versions of file
            List<File> previousFiles = fileRepository.GetFiles()
                .FindAll(f => f.Name == file.Name && f.ContentType == file.Type && f.DirectoryId.Equals(dirId));

            if (previousFiles.Count > 0)
            {
                File previousFile = previousFiles
                    .OrderBy(f => f.CreatedDate).First();
                newFile.ParentId = previousFile.Id;
                previousFile.IsCurrent = false;
                previousFile.CurrentId = newFile.Id;
                fileRepository.EditFile(previousFile);
                fileRepository.EditFile(newFile);
            }
        }
        public void DeleteFileFromService(FileDeleteParameterVM file)
        {
            string dirName;
            if (file.Directory.Equals("C:\\CloudProject"))
            {
                dirName = "";
            }
            else
            {
                dirName = file.Directory.Replace("C:\\", "");
            }
            string dirId;
            if (dirName.Equals(""))
            {
                dirId = "0";
            }
            else
            {
                dirId = directoryRepository.GetDirIdByName(dirName);
            }

            fileRepository.DeleteFileFromDirectory(file.Name, dirId);
        }
        internal void SendSignalToService(string fileId)
        {
            Socket socket;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8090;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            socket.Connect(ipEndPoint);
            byte[] byteData = Encoding.ASCII.GetBytes(fileId.ToCharArray());
            socket.Send(byteData, byteData.Length, 0);
        }
        public FileParameterVM GetFileToService(string fileId)
        {
            File file = fileRepository.GetFileById(fileId);

            string dirName = "";
            if (!file.DirectoryId.Equals("0"))
            {
                dirName = directoryRepository.GetCurrentDirName(file.DirectoryId);
            }

            string rootPath = @"D:\cloud-project-storage\";
            byte[] bytes = System.IO.File.ReadAllBytes(file.Path);
            FileParameterVM result = new FileParameterVM
            {
                Name = file.Name,
                Type = file.ContentType,
                Source = bytes,
                Directory = dirName
            };
            return result;
        }
    }
}