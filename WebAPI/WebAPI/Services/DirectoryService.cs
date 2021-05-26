using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IDirectoryRepository directoryRepository;
        public DirectoryService(IDirectoryRepository directoryRepository)
        {
            this.directoryRepository = directoryRepository;
        }
        public List<Directory> GetDirectories(string dirId)
        {
            return directoryRepository.GetDirectories(dirId);
        }
        public void AddDirectory(string dirId, string dirName)
        {
            directoryRepository.AddDirectory(dirId, dirName);
        }
        public string GetCurrentDirName(string dirId)
        {
            if (dirId.Equals("0"))
                return "Folder główny";
            else
                return directoryRepository.GetCurrentDirName(dirId);
        }
    }
}
