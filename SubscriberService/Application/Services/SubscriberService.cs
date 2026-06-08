using Common.Shared.Monitoring;
using SubscriberService.Application.DTO;
using SubscriberService.Application.Interfaces;
using SubscriberService.Application.Interfaces.Data;
using SubscriberService.Application.Interfaces.Queue;
using SubscriberService.Domain;

namespace SubscriberService.Application.Services;

public class SubscriberService(
    ISubscriberRepository repository,
    ISubscriberQueuePublisher queuePublisher,
    ILogger<SubscriberService> logger) : ISubscriberService
{
    public async Task<Subscriber> SubscribeAsync(SubscribeDto dto)
    {
        var existing = await repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                await repository.SetActiveAsync(existing.Id, true);
                logger.LogInformation("Re activated subscriber {SubscriberId}", existing.Id);
            }

            return existing;
        }

        var subscriber = new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(subscriber);

        var headers = new Dictionary<string, object?>();
        MessagingTraceContext.InjectTraceHeaders(headers);
        await queuePublisher.PublishNewSubscriberAsync(subscriber, headers);

        logger.LogInformation("Subscriber {SubscriberId} created and queued", subscriber.Id);
        return subscriber;
    }

    public async Task<bool> UnsubscribeAsync(Guid id)
    {
        var success = await repository.SetActiveAsync(id, false);
        if (success)
            logger.LogInformation("Subscriber {SubscriberId} unsubscribed", id);

        return success;
    }

    public async Task<List<Subscriber>> GetAllActiveAsync()
    {
        return await repository.GetAllActiveAsync();
    }
}
