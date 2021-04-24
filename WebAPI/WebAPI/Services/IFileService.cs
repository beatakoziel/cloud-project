using System.Collections.Generic;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Services
{
    public interface IFileService
    {
        List<FileVM> GetAllFiles();
        File GetFileById(string id);
        void AddFile(File data);
        void EditFile(File data);
        void DeleteFile(string id);
        string GetFileSource(string fileId);
    }
}
