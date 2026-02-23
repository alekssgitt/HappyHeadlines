using Microsoft.EntityFrameworkCore;
using ProfanityService.Application.Interfaces.Data;
using ProfanityService.Domain;

namespace ProfanityService.Infrastructure.Repositories;

public class ProfanityRepository(ProfanityDbContext context) : IProfanityRepository
{
    public async Task<List<ProfanityWord>> GetAllAsync()
    {
        return await context.ProfanityWords
            .AsNoTracking()
            .OrderBy(p => p.Word)
            .ToListAsync();
    }

    public async Task<ProfanityWord?> GetByWordAsync(string word)
    {
        return await context.ProfanityWords
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Word.ToLower() == word.ToLower());
    }

    public async Task<ProfanityWord> AddAsync(ProfanityWord profanityWord)
    {
        context.ProfanityWords.Add(profanityWord);
        await context.SaveChangesAsync();
        return profanityWord;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var word = await context.ProfanityWords.FirstOrDefaultAsync(p => p.Id == id);
        if (word is null) return false;

        context.ProfanityWords.Remove(word);
        await context.SaveChangesAsync();
        return true;
    }
}
