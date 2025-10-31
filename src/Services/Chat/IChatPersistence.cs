namespace pogadajmy_server.Services.Chat
{
    public interface IChatPersistence
    {
        ValueTask EnqueueAsync(Guid roomId, Guid userId, string text);
    }
}
