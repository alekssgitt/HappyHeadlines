using NewsletterService.Application.DTO;

namespace NewsletterService.Application.Interfaces;

public interface INewsletterService
{
    Task<List<ArticleSummaryDto>> GetDailyArticlesAsync(int count);
    List<ArticleSummaryDto> GetLatestQueuedArticles(int count);
}
