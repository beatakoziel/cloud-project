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
        FileVM GetCurrentFileByName(string name);
        void EditFile(File data);
        void DeleteFile(string id);
        List<FileVM> GetCurrentFiles(string dirId);
        List<File> GetFiles();
        List<File> GetFilesByName(string fileName);
        void DeleteFileFromDirectory(string fileName, string dirId);
        List<File> GetAllCurrentFiles();
    }
}
