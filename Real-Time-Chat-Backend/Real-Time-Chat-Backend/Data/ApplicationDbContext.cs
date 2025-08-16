using Microsoft.EntityFrameworkCore;
using Real_Time_Chat_Backend.Models;

namespace Real_Time_Chat_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Customer> Customer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Role);
            });

            // Configure Message entity
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Sender).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            });

            //Configure User entity
            modelBuilder.Entity<UserDetails>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.HasOne(e => e.User).
                WithMany(a => a.UserDetails).
                HasForeignKey(e => e.UserId).IsRequired();
            });

            //Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // Seed default user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123", // In production, use hashed passwords
                    Email = "admin@example.com",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
