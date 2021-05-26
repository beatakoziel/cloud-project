using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IDirectoryService
    {
        List<Directory> GetDirectories(string dirId);
        void AddDirectory(string dirId, string dirName);
        string GetCurrentDirName(string dirId);
    }
}
