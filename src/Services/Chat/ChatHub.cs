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
        private static string ConnKey(string connId) => $"conn:{connId}";
        public ChatHub(IConnectionMultiplexer redis /*, inne zależności */)
        {
            _redis = redis;
        }

        private Guid UserId() => Context.User!.GetSubGuid();
        
        // pomocnicze stałe
        private static string OnlineKey(string roomId) => $"online:{roomId}";
        private IDatabase R => _redis.GetDatabase();

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
            if (!Guid.TryParse(roomId, out _))
                throw new HubException("invalid_room_id");

            var userId = GetUserIdOrThrow();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await R.SetAddAsync(ConnKey(Context.ConnectionId), roomId);
            
            // presence ON
            await R.SetAddAsync(OnlineKey(roomId), userId.ToString());

            await Clients.Group(roomId)
                .SendAsync("Joined", new { userId, at = DateTime.UtcNow });
        }
        
        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            var userId = Context.User is null ? (Guid?)null : GetUserIdOrThrow();

            var rooms = await R.SetMembersAsync(ConnKey(Context.ConnectionId));
            foreach (var r in rooms)
            {
                var roomId = (string)r;
                if (userId.HasValue)
                    await R.SetRemoveAsync(OnlineKey(roomId), userId.Value.ToString());

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                await Clients.Group(roomId).SendAsync("Left", new { userId, at = DateTime.UtcNow });
            }
            await R.KeyDeleteAsync(ConnKey(Context.ConnectionId));

            await base.OnDisconnectedAsync(ex);
        }
        
        public async Task LeaveRoom(string roomId)
        {
            if (!Guid.TryParse(roomId, out _))
                throw new HubException("invalid_room_id");

            var userId = GetUserIdOrThrow();

            // presence OFF
            await R.SetRemoveAsync(OnlineKey(roomId), userId.ToString());

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId)
                .SendAsync("Left", new { userId, at = DateTime.UtcNow });
        }
        
        public async Task<string[]> GetOnline(string roomId)
        {
            if (!Guid.TryParse(roomId, out _))
                throw new HubException("invalid_room_id");

            var members = await R.SetMembersAsync(OnlineKey(roomId));
            return members.Select(x => (string)x).ToArray();
        }

        public async Task SendMessage(string roomId, string text)
        {
            if (!Guid.TryParse(roomId, out _))
                throw new HubException("invalid_room_id");

            var msg = text?.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            var userId = GetUserIdOrThrow();
            await Clients.Group(roomId).SendAsync("Message", new { userId, text = msg, at = DateTime.UtcNow });
        }

        public override Task OnConnectedAsync()
        {
            var claims = Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>();
            Console.WriteLine("[Hub] Claims: " + string.Join(", ", claims));
            return base.OnConnectedAsync();
        }
        
        public async Task Typing(string roomId, bool isTyping)
        {
            if (!Guid.TryParse(roomId, out _)) throw new HubException("invalid_room_id");
            var uid = GetUserIdOrThrow();
            var name = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? uid.ToString();

            await Clients.OthersInGroup(roomId).SendAsync("Typing", new {
                userId = uid,
                name,
                isTyping,
                at = DateTime.UtcNow
            });
        }
    }
}
