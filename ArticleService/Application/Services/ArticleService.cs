using ArticleService.Application.DTO;
using ArticleService.Application.Interfaces.Caching;
using ArticleService.Application.Interfaces;
using ArticleService.Application.Interfaces.Data;
using ArticleService.Domain;

namespace ArticleService.Application.Services;

public class ArticleService(
    IArticleRepository repository,
    IArticleCacheService cacheService,
    ILogger<ArticleService> logger) : IArticleService
{
    

    public async Task<Article> CreateAsync(CreateArticleDto dto)
    {
        var continent = dto.Continent.ToLowerInvariant().Trim();

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            Author = dto.Author,
            Continent = continent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(article, continent);

        if (continent == "global")
        {
            await cacheService.SetGlobalArticleAsync(article);
        }

        logger.LogInformation("Article {Id} created in shard '{Continent}'", article.Id, continent);

        return article;
    }

    public async Task<Article?> GetByIdAsync(Guid id, string? continent)
    {
        if (string.Equals(continent, "global", StringComparison.OrdinalIgnoreCase))
        {
            var cached = await cacheService.GetGlobalArticleByIdAsync(id);
            if (cached is not null)
            {
                cacheService.RecordArticleCacheHit();
                return cached;
            }

            cacheService.RecordArticleCacheMiss();
            var fromDb = await repository.GetByIdAsync(id, "global");
            if (fromDb is not null)
                await cacheService.SetGlobalArticleAsync(fromDb);

            return fromDb;
        }

        return await repository.GetByIdAsync(id, continent);
    }

    public async Task<List<Article>> GetAllAsync(string? continent)
    {
        if (string.Equals(continent, "global", StringComparison.OrdinalIgnoreCase))
        {
            var cached = await cacheService.GetRecentGlobalArticlesAsync();
            if (cached is not null)
            {
                cacheService.RecordArticleCacheHit();
                return cached;
            }

            cacheService.RecordArticleCacheMiss();
            var fromDb = await repository.GetRecentGlobalAsync(14);
            await cacheService.SetRecentGlobalArticlesAsync(fromDb);
            return fromDb;
        }

        return await repository.GetAllAsync(continent);
    }

    public async Task<Article?> UpdateAsync(Guid id, UpdateArticleDto dto, string? continent)
    {
        var updated = await repository.UpdateAsync(id, article =>
        {
            if (dto.Title is not null) article.Title = dto.Title;
            if (dto.Content is not null) article.Content = dto.Content;
            if (dto.Author is not null) article.Author = dto.Author;
        }, continent);

        if (updated is not null)
        {
            if (string.Equals(updated.Continent, "global", StringComparison.OrdinalIgnoreCase))
            {
                await cacheService.SetGlobalArticleAsync(updated);
            }
            logger.LogInformation("Article {Id} updated", id);
        }

        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, string? continent)
    {
        var deleted = await repository.DeleteAsync(id, continent);

        if (deleted)
        {
            if (string.Equals(continent, "global", StringComparison.OrdinalIgnoreCase))
            {
                await cacheService.RemoveGlobalArticleAsync(id);
            }
            logger.LogInformation("Article {Id} deleted", id);
        }

        return deleted;
    }
}
