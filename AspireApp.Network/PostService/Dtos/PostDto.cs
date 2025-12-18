using SharedObject;

namespace PostService.Dtos
{
    public class PostDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool LikedByMe { get; set; }
        public bool Typing { get; set; } = false;
        public int TotalLike { get; set; }
        public KeycloakUser? User { get; set; }
        public List<PhotoDto> Photos { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }
}
