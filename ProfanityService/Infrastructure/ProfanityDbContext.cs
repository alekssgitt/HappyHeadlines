using Microsoft.EntityFrameworkCore;
using ProfanityService.Domain;

namespace ProfanityService.Infrastructure;

public class ProfanityDbContext(DbContextOptions<ProfanityDbContext> options) : DbContext(options)
{
    public DbSet<ProfanityWord> ProfanityWords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfanityWord>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Word).IsRequired().HasMaxLength(100);
            entity.HasIndex(p => p.Word).IsUnique();
        });
    }
}
