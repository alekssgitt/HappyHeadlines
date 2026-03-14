using NewsletterService.Application.DTO;

namespace NewsletterService.Infrastructure.Queue;

public class LatestArticleBuffer
{
    private readonly LinkedList<ArticleSummaryDto> _articles = [];
    private readonly object _lock = new();

    public void Add(ArticleSummaryDto article)
    {
        lock (_lock)
        {
            _articles.AddFirst(article);
            while (_articles.Count > 200)
            {
                _articles.RemoveLast();
            }
        }
    }

    public List<ArticleSummaryDto> GetLatest(int count)
    {
        lock (_lock)
        {
            return _articles.Take(count).ToList();
        }
    }
}
