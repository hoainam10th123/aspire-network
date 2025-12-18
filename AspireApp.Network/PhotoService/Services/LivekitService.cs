using Livekit.Server.Sdk.Dotnet;
using System.Security.Claims;

namespace RoomService.Services
{
    public interface ILivekitService
    {
        Task<string> GetToken(string roomId);
    }
    public class LivekitService(IConfiguration config, IHttpContextAccessor httpContextAccessor) : ILivekitService
    {
        public Task<string> GetToken(string roomId) 
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userId))
            {
                throw new Exception("User not authenticated");
            }

            var token = new AccessToken(config["Livekit:apiKey"], config["Livekit:apiSecret"])
              .WithIdentity(userId)
              .WithName(userId)
              .WithGrants(new VideoGrants { RoomJoin = true, Room = roomId })
              //.WithAttributes(new Dictionary<string, string> { { "mykey", "myvalue" } })
              .WithTtl(TimeSpan.FromMinutes(30));

            var jwt = token.ToJwt();

            return Task.FromResult(jwt);
        }
    }
}
