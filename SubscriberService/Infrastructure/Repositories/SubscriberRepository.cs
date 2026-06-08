using Microsoft.EntityFrameworkCore;
using SubscriberService.Application.Interfaces.Data;
using SubscriberService.Domain;

namespace SubscriberService.Infrastructure.Repositories;

public class SubscriberRepository(SubscriberDbContext context) : ISubscriberRepository
{
    public async Task<Subscriber?> GetByEmailAsync(string email)
    {
        return await context.Subscribers
            .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
    }

    public async Task<Subscriber?> GetByIdAsync(Guid id)
    {
        return await context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Subscriber>> GetAllActiveAsync()
    {
        return await context.Subscribers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Subscriber> CreateAsync(Subscriber subscriber)
    {
        context.Subscribers.Add(subscriber);
        await context.SaveChangesAsync();
        return subscriber;
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive)
    {
        var existing = await context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
        if (existing is null) return false;

        existing.IsActive = isActive;
        existing.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }
}
