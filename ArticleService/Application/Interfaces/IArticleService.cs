using ArticleService.Application.DTO;
using ArticleService.Domain;

namespace ArticleService.Application.Interfaces;

public interface IArticleService
{
    Task<Article> CreateAsync(CreateArticleDto dto);
    Task<Article?> GetByIdAsync(Guid id, string? continent);
    Task<List<Article>> GetAllAsync(string? continent);
    Task<Article?> UpdateAsync(Guid id, UpdateArticleDto dto, string? continent);
    Task<bool> DeleteAsync(Guid id, string? continent);
}
