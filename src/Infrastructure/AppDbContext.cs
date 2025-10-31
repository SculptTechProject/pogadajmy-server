using Microsoft.EntityFrameworkCore;
using pogadajmy_server.Models;

namespace pogadajmy_server.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> User => Set<User>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<DmRoom> DmRooms => Set<DmRoom>();

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
                e.Property(u => u.City).HasMaxLength(15);
            });

            b.Entity<Room>(e =>
            {
                e.ToTable("rooms");
                e.HasKey(x => x.Id);
                e.Property(x => x.Type).HasMaxLength(10).IsRequired();
                e.Property(x => x.Name).HasMaxLength(80);
                e.Property(x => x.IsPrivate).IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                e.HasIndex(x => x.CreatedAt);
            });

            b.Entity<RoomMember>(e =>
            {
                e.ToTable("room_members");
                e.HasKey(x => new { x.RoomId, x.UserId });

                e.HasOne(x => x.Room)
                    .WithMany(r => r.Members)
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.User)
                    .WithMany(u => u.Memberships)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.JoinedAt).HasDefaultValueSql("now()");
                e.HasIndex(x => x.JoinedAt);
                e.HasIndex(x => x.UserId);
            });

            b.Entity<Message>(e =>
            {
                e.ToTable("messages");
                e.HasKey(x => x.Id);

                e.Property(x => x.Text).IsRequired().HasMaxLength(2000);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                e.HasIndex(x => new { x.RoomId, x.CreatedAt });

                e.HasOne(x => x.Room)
                    .WithMany(r => r.Messages)
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                e.HasOne(x => x.User)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            b.Entity<DmRoom>(e =>
            {
                e.ToTable("dm_rooms");
                e.HasKey(x => x.RoomId);
                e.Property(x => x.UserA).IsRequired();
                e.Property(x => x.UserB).IsRequired();

                // unikalność pary (UserA, UserB)
                e.HasIndex(x => new { x.UserA, x.UserB }).IsUnique();

                e.HasOne(x => x.Room)
                    .WithOne()
                    .HasForeignKey<DmRoom>(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                // jeśli chcesz FK do users, odkomentuj i dostosuj nawigacje w modelach:
                // e.HasOne<User>().WithMany().HasForeignKey(x => x.UserA).OnDelete(DeleteBehavior.Restrict);
                // e.HasOne<User>().WithMany().HasForeignKey(x => x.UserB).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
