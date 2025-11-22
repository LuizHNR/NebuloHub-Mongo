
using MongoDB.Bson.Serialization.Attributes;

namespace NebuloMongo.Application.DTOs.Request
{
    public class RequestReviewDto
    {
        public int Rating { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.Date;

        public string StartupId { get; set; }


    }
}
