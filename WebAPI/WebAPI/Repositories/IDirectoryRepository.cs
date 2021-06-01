using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface IDirectoryRepository
    {
        List<Directory> GetDirectories(string dirId);
        void AddDirectory(string dirId, string dirName);
        string GetCurrentDirName(string dirId);
        List<Directory> GetAllDirectories();
        string GetDirIdByName(string dirName);
    }
}
