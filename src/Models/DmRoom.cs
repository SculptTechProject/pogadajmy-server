namespace pogadajmy_server.Models
{
    public sealed class DmRoom
    {
        public Guid RoomId { get; set; }
        public Guid UserA  { get; set; }
        public Guid UserB  { get; set; }

        public Room? Room { get; set; }
        // public User? A { get; set; }
        // public User? B { get; set; }
    }
}
