using System.Text.Json.Serialization;

namespace SharedObject
{
    public class KeycloakUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
