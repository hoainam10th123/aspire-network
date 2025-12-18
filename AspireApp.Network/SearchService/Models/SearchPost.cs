using System.Text.Json.Serialization;

namespace SearchService.Models
{
    public class SearchPost
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;
        [JsonPropertyName("content")]
        public string Content { get; set; } = null!;
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;
        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; } = 0;
        [JsonPropertyName("lastUpdatedAt")]
        public long LastUpdatedAt { get; set; } = 0;
    }
}
