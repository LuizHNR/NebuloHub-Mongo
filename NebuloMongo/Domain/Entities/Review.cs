using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;

namespace NebuloMongo.Domain.Entities
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
        [BsonElement("comment")]
        public string Comment { get; set; }
        [BsonElement("date")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.Date;
        [BsonElement("startupId")]
        public string StartupId { get; set; }

        private Review(int rating, string userId, string comment, DateTime dataCriacao, string startupId)
        {
            Rating = rating;
            UserId = userId;
            Comment = comment;
            DataCriacao = dataCriacao;
            StartupId = startupId;
            

        }

        public void Atualizar(int rating, string userId, string comment, DateTime dataCriacao, string startupId)
        {
            Rating = rating;
            UserId = userId;
            Comment = comment;
            DataCriacao = dataCriacao;
            StartupId = startupId;

        }

        internal static Review Create(int rating, string userId, string comment, DateTime dataCriacao, string startupId)
        {
            return new Review( rating, userId,  comment, dataCriacao, startupId  );
        }

        public Review() { }
    }
}
