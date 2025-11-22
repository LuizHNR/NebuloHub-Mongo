
namespace NebuloMongo.Application.DTOs.Request
{
    public class RequestReviewDto
    {
        public string StartupId { get; set; }
        public string UserId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
