using ArticleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Infrastructure;

public class ArticleDbContext : DbContext
{
        public ArticleDbContext(DbContextOptions<ArticleDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(300);
                entity.Property(a => a.Content).IsRequired();
                entity.Property(a => a.Author).IsRequired().HasMaxLength(150);
                entity.Property(a => a.Continent).IsRequired().HasMaxLength(50);
            });
        }
}
