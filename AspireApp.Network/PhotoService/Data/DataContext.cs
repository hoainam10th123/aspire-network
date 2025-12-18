using Microsoft.EntityFrameworkCore;

namespace RoomService.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Models.Room> Rooms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
