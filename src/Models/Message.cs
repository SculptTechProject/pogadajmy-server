namespace pogadajmy_server.Models
{
    public sealed class Message
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Room Room { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
