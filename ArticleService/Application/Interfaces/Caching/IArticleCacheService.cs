using ArticleService.Domain;

namespace ArticleService.Application.Interfaces.Caching;

public interface IArticleCacheService
{
    Task<Article?> GetGlobalArticleByIdAsync(Guid id);
    Task SetGlobalArticleAsync(Article article);
    Task<List<Article>?> GetRecentGlobalArticlesAsync();
    Task SetRecentGlobalArticlesAsync(List<Article> articles);
    Task RemoveGlobalArticleAsync(Guid id);

    void RecordArticleCacheHit();
    void RecordArticleCacheMiss();
}
