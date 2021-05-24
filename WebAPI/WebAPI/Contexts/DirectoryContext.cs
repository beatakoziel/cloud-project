using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Contexts
{
    public class DirectoryContext
    {
        private readonly IMongoDatabase _database = null;
        public DirectoryContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.DatabaseName);
            }
        }
        public IMongoCollection<Directory> Directories
        {
            get
            {
                return _database.GetCollection<Directory>("Directories");
            }
        }
    }
}
