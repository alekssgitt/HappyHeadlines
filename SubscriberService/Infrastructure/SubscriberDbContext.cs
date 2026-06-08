using Microsoft.EntityFrameworkCore;
using SubscriberService.Domain;

namespace SubscriberService.Infrastructure;

public class SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : DbContext(options)
{
    public DbSet<Subscriber> Subscribers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.IsActive).IsRequired();
        });
    }
}
