using System;
using System.Collections.Generic;
using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.ViewModels;

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
        public File GetFileById(string id)
        {
            return fileRepository.GetFileById(id);
        }
        public void AddFile(File data)
        {
            fileRepository.AddFile(data);
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
    }
}
