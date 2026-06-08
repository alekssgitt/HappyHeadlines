using ArticleService.Application.Interfaces.Caching;
using ArticleService.Domain;
using Prometheus;
using StackExchange.Redis;
using System.Text.Json;

namespace ArticleService.Infrastructure.Caching;

public class ArticleCacheService(IConnectionMultiplexer redis) : IArticleCacheService
{
    private const string RecentGlobalKey = "article:global:recent14days";

    private static readonly Counter CacheHits = Metrics.CreateCounter(
        "article_cache_hits_total",
        "Total number of article cache hits.");

    private static readonly Counter CacheMisses = Metrics.CreateCounter(
        "article_cache_misses_total",
        "Total number of article cache misses.");

    private readonly IDatabase db = redis.GetDatabase();
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Article?> GetGlobalArticleByIdAsync(Guid id)
    {
        var value = await db.StringGetAsync($"article:global:{id}");
        return value.HasValue ? JsonSerializer.Deserialize<Article>(value!, JsonOptions) : null;
    }

    public async Task SetGlobalArticleAsync(Article article)
    {
        await db.StringSetAsync(
            $"article:global:{article.Id}",
            JsonSerializer.Serialize(article, JsonOptions),
            expiry: TimeSpan.FromDays(14));
    }

    public async Task<List<Article>?> GetRecentGlobalArticlesAsync()
    {
        var value = await db.StringGetAsync(RecentGlobalKey);
        return value.HasValue ? JsonSerializer.Deserialize<List<Article>>(value!, JsonOptions) : null;
    }

    public async Task SetRecentGlobalArticlesAsync(List<Article> articles)
    {
        await db.StringSetAsync(
            RecentGlobalKey,
            JsonSerializer.Serialize(articles, JsonOptions),
            expiry: TimeSpan.FromMinutes(15));
    }

    public async Task RemoveGlobalArticleAsync(Guid id)
    {
        await db.KeyDeleteAsync($"article:global:{id}");
    }

    public void RecordArticleCacheHit() => CacheHits.Inc();
    public void RecordArticleCacheMiss() => CacheMisses.Inc();
}
