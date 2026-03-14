using Common.Shared.Monitoring;
using PublisherService.Application.DTO;
using PublisherService.Application.Interfaces;
using PublisherService.Application.Interfaces.Queue;
using System.Text.Json;

namespace PublisherService.Application.Services;

public class PublisherService(IArticleQueuePublisher queuePublisher, ILogger<PublisherService> logger) : IPublisherService
{
    public async Task<Guid> PublishAsync(PublishArticleDto dto)
    {
        var articleId = Guid.NewGuid();

        var message = new
        {
            Id = articleId,
            Title = dto.Title,
            Content = dto.Content,
            Author = dto.Author,
            Continent = dto.Continent.ToLowerInvariant().Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var headers = new Dictionary<string, object?>();
        MessagingTraceContext.InjectTraceHeaders(headers);

        await queuePublisher.PublishAsync(JsonSerializer.Serialize(message), headers);
        logger.LogInformation("Published article {ArticleId} to article queue", articleId);

        return articleId;
    }
}
