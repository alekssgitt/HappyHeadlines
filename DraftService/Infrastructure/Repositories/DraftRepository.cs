using DraftService.Application.Interfaces.Data;
using DraftService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Infrastructure.Repositories;

public class DraftRepository(DraftDbContext context) : IDraftRepository
{
    public async Task<Draft> CreateAsync(Draft draft)
    {
        context.Drafts.Add(draft);
        await context.SaveChangesAsync();
        return draft;
    }

    public async Task<Draft?> GetByIdAsync(Guid id)
    {
        return await context.Drafts.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Draft>> GetAllAsync()
    {
        return await context.Drafts
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Draft?> UpdateAsync(Guid id, Action<Draft> applyUpdates)
    {
        var draft = await context.Drafts.FirstOrDefaultAsync(d => d.Id == id);
        if (draft is null) return null;

        applyUpdates(draft);
        draft.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return draft;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var draft = await context.Drafts.FirstOrDefaultAsync(d => d.Id == id);
        if (draft is null) return false;

        context.Drafts.Remove(draft);
        await context.SaveChangesAsync();
        return true;
    }
}
