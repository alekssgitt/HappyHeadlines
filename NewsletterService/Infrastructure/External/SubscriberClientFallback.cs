using NewsletterService.Application.DTO;
using NewsletterService.Application.Interfaces;

namespace NewsletterService.Infrastructure.External;

public class SubscriberClientFallback(ISubscriberClient inner, ILogger<SubscriberClientFallback> logger) : ISubscriberClient
{
    public async Task<List<SubscriberDto>> GetAllActiveSubscribersAsync()
    {
        try
        {
            return await inner.GetAllActiveSubscribersAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "SubscriberService unavailable — circuit breaker fallback: returning empty subscriber list");
            return [];
        }
    }
}
