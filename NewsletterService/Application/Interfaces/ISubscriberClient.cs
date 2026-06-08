using NewsletterService.Application.DTO;

namespace NewsletterService.Application.Interfaces;

public interface ISubscriberClient
{
    Task<List<SubscriberDto>> GetAllActiveSubscribersAsync();
}
