using MongoDB.Bson.Serialization.Attributes;
using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Application.DTOs.Response
{
    public class ResponseReviewDto
    {
        public string Id { get; set; }

        public int Rating { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.Date;

        public string StartupId { get; set; }


        public static ResponseReviewDto FromEntity(Review u)
        {
            return new ResponseReviewDto
            {
                Id = u.Id,
                Rating = u.Rating,
                UserId = u.UserId,
                Comment = u.Comment,
                DataCriacao = u.DataCriacao,
                StartupId = u.StartupId
                
            };
        }

    }
}
