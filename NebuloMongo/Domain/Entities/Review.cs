using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NebuloMongo.Domain.Entities
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string StartupId { get; set; }
        public string UserId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
