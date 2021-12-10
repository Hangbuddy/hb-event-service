using Microsoft.EntityFrameworkCore;
using EventService.Models;

namespace EventService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventUser>()
                .HasKey(nameof(EventUser.Event), nameof(EventUser.UserId));

            modelBuilder.Entity<Event>()
                .HasMany(c => c.EventUsers)
                .WithOne(e => e.Event)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
    }
}