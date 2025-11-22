using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NebuloMongo.Application.Settings;
using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Infrastructure.Context
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<MongoSettings> settings)
        {
            var cfg = settings.Value;

            var client = new MongoClient(cfg.ConnectionString);
            _database = client.GetDatabase(cfg.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Startup> Startups => _database.GetCollection<Startup>("startups");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
