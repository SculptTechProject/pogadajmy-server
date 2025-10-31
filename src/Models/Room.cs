namespace pogadajmy_server.Models
{
    public sealed class Room
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "group";
        public string? Name { get; set; }
        public bool IsPrivate { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RoomMember> Members { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
