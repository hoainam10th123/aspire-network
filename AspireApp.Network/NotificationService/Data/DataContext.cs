using Microsoft.EntityFrameworkCore;

namespace NotificationService.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
