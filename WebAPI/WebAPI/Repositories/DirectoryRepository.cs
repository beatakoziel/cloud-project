using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Contexts;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly DirectoryContext _context = null;

        public DirectoryRepository(IOptions<Settings> settings)
        {
            _context = new DirectoryContext(settings);
        }
        public List<Directory> GetDirectories(string dirId)
        {
            return _context.Directories.Find(x => x.ParentId.Equals(dirId)).ToList();
        }
        public void AddDirectory(string dirId, string dirName)
        {
            Directory directory = new Directory
            {
                Name = dirName,
                ParentId = dirId
            };
            _context.Directories.InsertOne(directory);
        }
        public string GetCurrentDirName(string dirId)
        {
            Directory dir = _context.Directories.Find(x => x.Id.Equals(dirId)).FirstOrDefault();
            return dir.Name;
        }
        public List<Directory> GetAllDirectories()
        {
            return _context.Directories.Find(x => true).ToList();
        }
        public string GetDirIdByName(string dirName)
        {
            Directory directory = _context.Directories.Find(x => x.Name.Equals(dirName)).FirstOrDefault();
            return directory.Id;
        }
    }
}
