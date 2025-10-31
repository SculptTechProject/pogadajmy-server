namespace pogadajmy_server.Models
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsPro { get; set; }
        public bool Active { get; set; } = true;
        public string? City { get; set; }

        public ICollection<RoomMember> Memberships { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
