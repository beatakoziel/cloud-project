using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public FileService(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        public List<FileVM> GetAllFiles()
        {
            return fileRepository.GetAllFiles();
        }

        public List<FileVM> GetCurrentFiles()
        {
            return fileRepository.GetCurrentFiles();
        }

        public File GetFileById(string id)
        {
            return fileRepository.GetFileById(id);
        }
        
        public FileVM GetCurrentFile(string name)
        {
            return fileRepository.GetCurrentFileByName(name);
        }

        public void AddFile(IFormFile data)
        {
            string rootPath = "C:\\Users\\beatinho\\Downloads\\uploads\\";
            string filePath = rootPath + data.FileName;
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                data.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<File> previousFiles = fileRepository.GetFiles()
                .FindAll(f => f.Name == data.FileName && f.ContentType == data.ContentType);
            File newFile = new File();
            newFile.Name = data.FileName;
            newFile.Path = filePath;
            newFile.ContentType = data.ContentType;
            newFile.IsCurrent = true;
            newFile.CreatedDate = DateTime.Now;
            fileRepository.AddFile(newFile);
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

        public void EditFile(File data)
        {
            fileRepository.EditFile(data);
        }

        public void DeleteFile(string id)
        {
            fileRepository.DeleteFile(id);
        }

        public string GetFileSource(string fileId)
        {
            File file = GetFileById(fileId);
            string source = file.Source;
            int index = source.IndexOf(',');
            return source.Remove(0, index + 1);
        }

        public void DeleteFileByName(string fileName)
        {
            List<File> files = fileRepository.GetFilesByName(fileName);
            foreach (var file in files)
            {
                DeleteFile(file.Id);
            }
        }
    }
}