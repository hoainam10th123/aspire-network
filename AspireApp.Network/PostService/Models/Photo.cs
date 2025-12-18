using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PostService.Models
{
    public class Photo
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PostId { get; set; } = null!;
        public string Url { get; set; } = null!;

        [JsonIgnore]
        public Post Post { get; set; } = null!;
    }
}
