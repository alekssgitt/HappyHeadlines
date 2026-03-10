using DraftService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Infrastructure;

public class DraftDbContext(DbContextOptions<DraftDbContext> options) : DbContext(options)
{
    public DbSet<Draft> Drafts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Draft>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Title).IsRequired().HasMaxLength(300);
            entity.Property(d => d.Content).IsRequired();
            entity.Property(d => d.Author).IsRequired().HasMaxLength(150);
            entity.Property(d => d.Status).IsRequired().HasMaxLength(50);
        });
    }
}
