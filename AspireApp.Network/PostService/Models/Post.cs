using SharedObject;
using System.ComponentModel.DataAnnotations;

namespace PostService.Models
{
    public class Post : IUpdatable
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        [Required]
        public string UserId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public int TotalLike { get; set; } = 0;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    }
}
