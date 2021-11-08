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
                .HasKey(nameof(EventUser.EventId), nameof(EventUser.UserId));
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
    }
}