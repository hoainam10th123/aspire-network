using Microsoft.EntityFrameworkCore;
using PostService.Models;
using SharedObject;

namespace PostService.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostLike>().HasKey(pl => new { pl.PostId, pl.UserId });    // Composite Key

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                    e.State == EntityState.Modified &&
                    e.Entity is IUpdatable
                );

            foreach (var entry in entries)
            {               
                if (entry.State == EntityState.Modified)
                {
                    var entity = (IUpdatable)entry.Entity;
                    entity.LastUpdatedAt = DateTime.UtcNow;
                }                               
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
