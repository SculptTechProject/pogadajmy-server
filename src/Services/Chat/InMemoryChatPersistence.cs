using pogadajmy_server.Infrastructure;
using pogadajmy_server.Models;
using System.Threading.Channels;

namespace pogadajmy_server.Services.Chat
{
    public sealed class InMemoryChatPersistence : BackgroundService, IChatPersistence
    {
        private readonly Channel<(Guid roomId, Guid userId, string text)> _ch =
            Channel.CreateUnbounded<(Guid, Guid, string)>();
        private readonly IServiceProvider _sp;

        public InMemoryChatPersistence(IServiceProvider sp) => _sp = sp;

        public ValueTask EnqueueAsync(Guid roomId, Guid userId, string text)
            => _ch.Writer.WriteAsync((roomId, userId, text));

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (await _ch.Reader.WaitToReadAsync(ct))
            {
                while (_ch.Reader.TryRead(out var item))
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Messages.Add(new Message { RoomId = item.roomId, UserId = item.userId, Text = item.text });
                    await db.SaveChangesAsync(ct);
                }
            }
        }
    }
}
