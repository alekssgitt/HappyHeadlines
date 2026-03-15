using CommentService.Application.Interfaces.Caching;
using CommentService.Domain;
using Prometheus;
using StackExchange.Redis;
using System.Text.Json;

namespace CommentService.Infrastructure.Caching;

public class CommentCacheService(IConnectionMultiplexer redis) : ICommentCacheService
{
    private const string LruKey = "comment:lru:articles";
    private const int MaxArticles = 30;

    private static readonly Counter CacheHits = Metrics.CreateCounter(
        "comment_cache_hits_total",
        "Total number of comment cache hits.");

    private static readonly Counter CacheMisses = Metrics.CreateCounter(
        "comment_cache_misses_total",
        "Total number of comment cache misses.");

    private readonly IDatabase db = redis.GetDatabase();
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<List<Comment>?> GetCommentsByArticleIdAsync(Guid articleId)
    {
        var key = GetArticleCommentsKey(articleId);
        var value = await db.StringGetAsync(key);
        if (!value.HasValue) return null;

        await TouchLruAsync(articleId);
        return JsonSerializer.Deserialize<List<Comment>>(value!, JsonOptions);
    }

    public async Task SetCommentsByArticleIdAsync(Guid articleId, List<Comment> comments)
    {
        var key = GetArticleCommentsKey(articleId);
        await db.StringSetAsync(
            key,
            JsonSerializer.Serialize(comments, JsonOptions),
            expiry: TimeSpan.FromHours(12));

        await TouchLruAsync(articleId);
        await EnforceLruLimitAsync();
    }

    public async Task InvalidateArticleCommentsAsync(Guid articleId)
    {
        await db.KeyDeleteAsync(GetArticleCommentsKey(articleId));
    }

    public void RecordCommentCacheHit() => CacheHits.Inc();
    public void RecordCommentCacheMiss() => CacheMisses.Inc();

    private async Task TouchLruAsync(Guid articleId)
    {
        await db.SortedSetAddAsync(LruKey, articleId.ToString(), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    private async Task EnforceLruLimitAsync()
    {
        var size = await db.SortedSetLengthAsync(LruKey);
        if (size <= MaxArticles) return;

        var toEvictCount = size - MaxArticles;
        var oldest = await db.SortedSetRangeByRankAsync(LruKey, 0, toEvictCount - 1, Order.Ascending);
        foreach (var articleIdValue in oldest)
        {
            if (!Guid.TryParse(articleIdValue, out var articleId)) continue;

            await db.KeyDeleteAsync(GetArticleCommentsKey(articleId));
            await db.SortedSetRemoveAsync(LruKey, articleIdValue);
        }
    }

    private static string GetArticleCommentsKey(Guid articleId) => $"comment:article:{articleId}";
}
