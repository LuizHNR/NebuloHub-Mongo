namespace NebuloMongo.Application.DTOs.Response
{
    public class ResponseReviewDto
    {
        public string Id { get; set; }

        public string StartupId { get; set; }
        public string UserId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
