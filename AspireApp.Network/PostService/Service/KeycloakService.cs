using Microsoft.Extensions.Caching.Memory;
using PostService.Dtos;
using SharedObject;

namespace PostService.Service
{
    public interface IKeycloakService
    {
        Task<KeycloakToken?> GetAccessTokenAsync();
        Task<KeycloakUser?> GetUserByUserIdAsync(string userId);
    }
    public class KeycloakService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache) : IKeycloakService
    {
        public async Task<KeycloakToken?> GetAccessTokenAsync()
        {
            using var client = new HttpClient();

            var url = "http://localhost:8080/realms/network/protocol/openid-connect/token";

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", "nextjs" },
                { "client_secret", "Wk5XZXozSmZjVEYKGowdnh8OPVmHt80o" },
                { "scope", "openid" }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadFromJsonAsync<KeycloakToken>();

            return result;
        }

        public async Task<KeycloakUser?> GetUserByUserIdAsync(string userId)
        {
            using var client = httpClientFactory.CreateClient();

            var token = await memoryCache.GetOrCreateAsync(
                "token",
                async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(4);
                    var token = await GetAccessTokenAsync();
                    return token;
                });         

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await client.GetAsync($"http://localhost:8080/admin/realms/network/users/{userId}");
            var result = await response.Content.ReadFromJsonAsync<KeycloakUser>();
            return result;
        }
    }
}
