using SubscriberService.Application.DTO;
using SubscriberService.Domain;

namespace SubscriberService.Application.Interfaces;

public interface ISubscriberService
{
    Task<Subscriber> SubscribeAsync(SubscribeDto dto);
    Task<bool> UnsubscribeAsync(Guid id);
    Task<List<Subscriber>> GetAllActiveAsync();
}
