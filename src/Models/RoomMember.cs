namespace pogadajmy_server.Models
{
    public sealed class RoomMember
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public Room Room { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
