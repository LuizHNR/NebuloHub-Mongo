namespace NebuloMongo.Application.DTOs.Request
{
    public class RequestStartupDto
    {

        public string OwnerId { get; set; }
        public string Name { get; set; }
        public List<string> Skills { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
