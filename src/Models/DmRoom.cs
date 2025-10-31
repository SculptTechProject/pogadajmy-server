namespace pogadajmy_server.Models
{
    public sealed class DmRoom
    {
        public Guid RoomId { get; set; }
        public Guid UserA  { get; set; }
        public Guid UserB  { get; set; }

        // Nawigacje (opcjonalnie, jeśli masz encję User/Room)
        public Room? Room { get; set; }
        // public User? A { get; set; }
        // public User? B { get; set; }
    }
}
