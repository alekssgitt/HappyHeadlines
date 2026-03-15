using ArticleService.Application.Interfaces.Caching;
using ArticleService.Application.Interfaces.Data;

namespace ArticleService.Infrastructure.Caching;

public class ArticleCachePreloadWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ArticleCachePreloadWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                var cache = scope.ServiceProvider.GetRequiredService<IArticleCacheService>();

                var recentArticles = await repository.GetRecentGlobalAsync(14);
                await cache.SetRecentGlobalArticlesAsync(recentArticles);

                foreach (var article in recentArticles)
                {
                    await cache.SetGlobalArticleAsync(article);
                }

                logger.LogInformation("ArticleCache offline preload completed with {Count} global articles", recentArticles.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ArticleCache offline preload failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
