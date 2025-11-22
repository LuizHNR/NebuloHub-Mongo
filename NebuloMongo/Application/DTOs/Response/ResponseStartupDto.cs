namespace NebuloMongo.Application.DTOs.Response
{
    public class ResponseStartupDto
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }
        public string Name { get; set; }
        public List<string> Skills { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
