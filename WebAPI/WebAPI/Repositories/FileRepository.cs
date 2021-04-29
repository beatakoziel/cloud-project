﻿using Microsoft.Extensions.Options;
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
                Type = x.Type
            }).ToList();

            return result;
        }
        public File GetFileById(string id)
        {
            return _context.Files.Find(x => x.Id == id).FirstOrDefault();
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
    }
}
