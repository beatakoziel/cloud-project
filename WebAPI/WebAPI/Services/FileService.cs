using System.Collections.Generic;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        public FileService(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }
        public List<File> GetAllFiles()
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
    }
}
