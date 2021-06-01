using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Contexts;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly FileContext _context = null;

        public FileRepository(IOptions<Settings> settings)
        {
            _context = new FileContext(settings);
        }

        public List<FileVM> GetAllFiles()
        {
            List<File> files = _context.Files.Find(x => true).ToList();
            List<FileVM> result = files.Select(x => new FileVM
            {
                Id = x.Id,
                Name = x.Name,
                ContentType = x.ContentType
            }).ToList();

            return result;
        }

        public List<FileVM> GetCurrentFiles(string dirId)
        {
            List<File> files = _context.Files.Find(x => x.IsCurrent == true && x.DirectoryId.Equals(dirId)).ToList();
            List<FileVM> result = files.Select(x => new FileVM
            {
                Id = x.Id,
                Name = x.Name,
                ContentType = x.ContentType,
                CreatedDate = x.CreatedDate.ToLocalTime()
            }).ToList();


            foreach (var res in result)
            {
                List<FileVM> previousVersions = _context.Files
                .Find(x => x.Name.Equals(res.Name) && x.DirectoryId.Equals(dirId) && !x.IsCurrent)
                .ToList()
                .Select(f => new FileVM
                {
                    Id = f.Id,
                    CreatedDate = f.CreatedDate.ToLocalTime(),
                    Name = res.Name
                }).ToList();
                res.PreviousVersions = previousVersions;
            }

            return result;
        }

        public List<File> GetFiles()
        {
            return _context.Files.Find(x => true).ToList();
        }

        public File GetFileById(string id)
        {
            return _context.Files.Find(x => x.Id == id).FirstOrDefault();
        }

        public FileVM GetCurrentFileByName(string name)
        {
            List<File> files = _context.Files.Find(x => x.Name == name && x.IsCurrent)
                .ToList();
            if (files.Count < 1)
                return null;
            File file = files.First();
            List<FileVM> previousVersions = _context.Files
                .Find(x => x.Name.Equals(name) && !x.IsCurrent)
                .ToList()
                .Select(f => new FileVM
                {
                    Id = f.Id,
                    CreatedDate = f.CreatedDate.ToLocalTime()
                }).ToList();
            return new FileVM
            {
                Id = file.Id,
                Name = file.Name,
                ContentType = file.ContentType,
                CreatedDate = file.CreatedDate.ToLocalTime(),
                PreviousVersions = previousVersions
            };
        }

        public void AddFile(File data)
        {
            _context.Files.InsertOne(data);
        }

        public void EditFile(File data)
        {
            _context.Files.ReplaceOne(x => x.Id == data.Id, data);
        }

        public void DeleteFile(string id)
        {
            _context.Files.DeleteOne(x => x.Id == id);
        }

        public List<File> GetFilesByName(string fileName)
        {
            return _context.Files.Find(x => x.Name.Equals(fileName)).ToList();
        }
        public void DeleteFileFromDirectory(string fileName, string dirId)
        {
            _context.Files.DeleteOne(x => x.Name.Equals(fileName) && x.DirectoryId.Equals(dirId));
        }
    }
}