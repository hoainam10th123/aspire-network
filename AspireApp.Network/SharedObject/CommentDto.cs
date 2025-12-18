

namespace SharedObject
{
    public class CommentDto
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public KeycloakUser? User { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string PostId { get; set; } = string.Empty;
    }
}
