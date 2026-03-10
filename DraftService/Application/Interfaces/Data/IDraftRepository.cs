using DraftService.Domain;

namespace DraftService.Application.Interfaces.Data;

public interface IDraftRepository
{
    Task<Draft> CreateAsync(Draft draft);
    Task<Draft?> GetByIdAsync(Guid id);
    Task<List<Draft>> GetAllAsync();
    Task<Draft?> UpdateAsync(Guid id, Action<Draft> applyUpdates);
    Task<bool> DeleteAsync(Guid id);
}
