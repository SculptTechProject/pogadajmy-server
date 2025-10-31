using Microsoft.EntityFrameworkCore;
using pogadajmy_server.Models;

namespace pogadajmy_server.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> User => Set<User>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(u => u.Id);
                e.Property(u => u.Name).IsRequired().HasMaxLength(15);
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.IsPro).IsRequired();
                e.Property(u => u.Active);
                e.Property(u => u.City).IsRequired(false).HasMaxLength(15);
            });
        }
    }
}
