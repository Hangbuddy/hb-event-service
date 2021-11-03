using Microsoft.EntityFrameworkCore;
using EventService.Models;

namespace EventService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
    }
}