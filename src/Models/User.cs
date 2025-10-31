namespace pogadajmy_server.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsPro { get; set; } = false;
        public bool Active { get; set; } = default!;
        public string? City { get; set; }
        
    }
}
