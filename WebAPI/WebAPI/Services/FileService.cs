using System;
using System.Collections.Generic;
using System.IO;
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
            File newFile = new File();
            List<File> files = fileRepository.GetFiles();
            foreach(var file in files)
            {
                if(file.Name.Equals(data.Name))
                {
                    file.IsCurrent = false;
                    fileRepository.EditFile(file);
                }
            }
            newFile.Name = data.FileName;
            newFile.Path = filePath;
            newFile.ContentType = data.ContentType;
            newFile.IsCurrent = true;
            newFile.CreatedDate = DateTime.Now;
            fileRepository.AddFile(newFile);
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
            foreach(var file in files)
            {
                DeleteFile(file.Id);
            }
        }
    }
}
