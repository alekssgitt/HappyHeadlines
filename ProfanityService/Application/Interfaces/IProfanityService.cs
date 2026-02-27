using ProfanityService.Application.DTO;
using ProfanityService.Domain;

namespace ProfanityService.Application.Interfaces;

public interface IProfanityService
{
    Task<CheckTextResultDto> CheckTextAsync(CheckTextDto dto);
    Task<List<ProfanityWord>> GetAllWordsAsync();
    Task<ProfanityWord> AddWordAsync(AddWordDto dto);
    Task<bool> DeleteWordAsync(Guid id);
}
