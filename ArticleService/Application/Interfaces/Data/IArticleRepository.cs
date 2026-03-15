

using ArticleService.Domain;

namespace ArticleService.Application.Interfaces.Data;

public interface IArticleRepository
{
    Task<Article> CreateAsync(Article article, string continent);
    Task<Article?> GetByIdAsync(Guid id, string? continent);
    Task<List<Article>> GetAllAsync(string? continent);
    Task<List<Article>> GetRecentGlobalAsync(int days);
    Task<Article?> UpdateAsync(Guid id, Action<Article> applyUpdates, string? continent);
    Task<bool> DeleteAsync(Guid id, string? continent);
}
