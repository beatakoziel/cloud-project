using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Services
{
    public interface IFileService
    {
        List<FileVM> GetAllFiles();
        File GetFileById(string id);
        void AddFile(IFormFile data);
        void EditFile(File data);
        void DeleteFile(string id);
        string GetFileSource(string fileId);
        List<FileVM> GetCurrentFiles();
        void DeleteFileByName(string fileName);
    }
}
