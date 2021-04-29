using System.Collections.Generic;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Repositories
{
    public interface IFileRepository
    {
        List<FileVM> GetAllFiles();
        File GetFileById(string id);
        void AddFile(File data);
        void EditFile(File data);
        void DeleteFile(string id);
        List<FileVM> GetCurrentFiles();
        List<File> GetFiles();
        List<File> GetFilesByName(string fileName);
    }
}
