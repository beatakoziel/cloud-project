using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Contexts
{
    public class FileContext
    {
        private readonly IMongoDatabase _database = null;
        public FileContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if(client != null)
            {
                _database = client.GetDatabase(settings.Value.DatabaseName);
            }
        }
        public IMongoCollection<File> Files
        {
            get
            {
                return _database.GetCollection<File>("Files");
            }
        }
    }
}
