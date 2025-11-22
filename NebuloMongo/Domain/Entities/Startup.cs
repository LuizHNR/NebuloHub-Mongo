using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NebuloMongo.Domain.Entities
{
    public class Startup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string OwnerId { get; set; }
        public string Name { get; set; }
        public List<string> Skills { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
