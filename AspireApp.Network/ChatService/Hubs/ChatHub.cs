using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Typing(string postId)
        {
            await Clients.Others.SendAsync("Typing", postId, true);
        }

        public async Task StopTyping(string postId)
        {
            await Clients.Others.SendAsync("Typing", postId, false);
        }
    }
}
