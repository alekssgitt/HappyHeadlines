using NewsletterService.Application.DTO;
using NewsletterService.Application.Interfaces;
using NewsletterService.Infrastructure.Queue;
using System.Net.Http.Json;

namespace NewsletterService.Application.Services;

public class NewsletterService(
    IHttpClientFactory httpClientFactory,
    LatestArticleBuffer buffer,
    ILogger<NewsletterService> logger) : INewsletterService
{
    private const string GetAllArticlesRoute = "/api/articles/get-all-articles";
    private const string ArticleServiceClientName = "ArticleServiceClient";

    public async Task<List<ArticleSummaryDto>> GetDailyArticlesAsync(int count)
    {
        var httpClient = httpClientFactory.CreateClient(ArticleServiceClientName);
        var articles = await httpClient.GetFromJsonAsync<List<ArticleSummaryDto>>(GetAllArticlesRoute) ?? [];
        var selected = articles
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToList();

        logger.LogInformation("Prepared {Count} articles for daily newsletter", selected.Count);
        return selected;
    }

    public List<ArticleSummaryDto> GetLatestQueuedArticles(int count)
    {
        var latest = buffer.GetLatest(count);
        logger.LogInformation("Loaded {Count} latest queued newsletter articles", latest.Count);
        return latest;
    }
}
