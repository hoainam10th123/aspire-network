using System.Text.Json.Serialization;

namespace PostService.Dtos
{
    public class KeycloakToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
