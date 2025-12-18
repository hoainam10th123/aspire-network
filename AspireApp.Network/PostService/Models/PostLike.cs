
namespace PostService.Models
{
    public class PostLike
    {
        public string PostId { get; set; } = null!;
        public string UserId { get; set; } = null!;        
        public Post Post { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
