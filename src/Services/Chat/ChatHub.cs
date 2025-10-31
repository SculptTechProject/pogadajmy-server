using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using pogadajmy_server.Infrastructure;
using pogadajmy_server.Utils;
using StackExchange.Redis;

namespace pogadajmy_server.Services.Chat
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IConnectionMultiplexer _redis;
        public ChatHub(IConnectionMultiplexer redis /*, inne zależności */)
        {
            _redis = redis;
        }

        private Guid UserId() => Context.User!.GetSubGuid();
        private Guid GetUserIdOrThrow()
        {
            var sub = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(sub, out var id))
                throw new HubException("unauthorized"); 

            return id;
        }

        public async Task JoinRoom(string roomId)
        {
            if (!Guid.TryParse(roomId, out var rid))
                throw new HubException("invalid_room_id");

            var userId = GetUserIdOrThrow();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("Joined", new { userId, at = DateTime.UtcNow });
        }

        public async Task SendMessage(string roomId, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var userId = GetUserIdOrThrow();
            await Clients.Group(roomId).SendAsync("Message", new { userId, text, at = DateTime.UtcNow });
        }

        public override Task OnConnectedAsync()
        {
            var claims = Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>();
            Console.WriteLine("[Hub] Claims: " + string.Join(", ", claims));
            return base.OnConnectedAsync();
        }
        
        public async Task Typing(string roomId, bool isTyping)
        {
            var uid = UserId();
            await Clients.OthersInGroup(roomId).SendAsync("Typing", new { userId = uid, isTyping, at = DateTime.UtcNow });
        }
    }
}
