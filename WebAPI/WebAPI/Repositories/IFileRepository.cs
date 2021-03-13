using System.Collections.Generic;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface IFileRepository
    {
        List<File> GetAllFiles();
        File GetFileById(string id);
        void AddFile(File data);
        void EditFile(File data);
        void DeleteFile(string id);
    }
}
