using CommentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure;

public class CommentDbContext(DbContextOptions<CommentDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ArticleId).IsRequired();
            entity.Property(c => c.Author).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Content).IsRequired();
            entity.HasIndex(c => c.ArticleId);
        });
    }
}
