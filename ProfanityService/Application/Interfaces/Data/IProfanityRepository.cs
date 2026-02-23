using ProfanityService.Domain;

namespace ProfanityService.Application.Interfaces.Data;

public interface IProfanityRepository
{
    Task<List<ProfanityWord>> GetAllAsync();
    Task<ProfanityWord?> GetByWordAsync(string word);
    Task<ProfanityWord> AddAsync(ProfanityWord profanityWord);
    Task<bool> DeleteAsync(Guid id);
}
