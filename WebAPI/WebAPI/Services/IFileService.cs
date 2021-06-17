using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using WebAPI.ViewModels;
using WebAPI.ViewModels.ServiceConnection;
using File = WebAPI.Models.File;

namespace WebAPI.Services
{
    public interface IFileService
    {
        List<FileVM> GetAllFiles();
        File GetFileById(string id);
        FileVM GetCurrentFile(string name);
        void AddFile(IFormFile data, string dirId);
        void EditFile(File data);
        void DeleteFile(string id);
        FileSourceVM GetFileSource(string fileId);
        List<FileVM> GetCurrentFiles(string dirId);
        void DeleteFileByName(string fileName);
        void AddFileFromService(FileParameterVM file);
        void DeleteFileFromService(FileDeleteParameterVM file);

        FileParameterVM GetFileToService(string fileId);
        List<FileParameterVM> SynchronizeDirectory();
    }
}
