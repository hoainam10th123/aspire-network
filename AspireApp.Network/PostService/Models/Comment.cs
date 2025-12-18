using SharedObject;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PostService.Models
{
    public class Comment : IUpdatable
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();        
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public string PostId { get; set; } = string.Empty;

        [JsonIgnore]
        public Post Post { get; set; } = null!;
    }
}
