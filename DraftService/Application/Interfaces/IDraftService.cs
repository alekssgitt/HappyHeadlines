using DraftService.Application.DTO;
using DraftService.Domain;

namespace DraftService.Application.Interfaces;

public interface IDraftService
{
    Task<Draft> CreateAsync(CreateDraftDto dto);
    Task<Draft?> GetByIdAsync(Guid id);
    Task<List<Draft>> GetAllAsync();
    Task<Draft?> UpdateAsync(Guid id, UpdateDraftDto dto);
    Task<bool> DeleteAsync(Guid id);
}
