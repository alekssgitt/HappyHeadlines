using SubscriberService.Domain;

namespace SubscriberService.Application.Interfaces.Data;

public interface ISubscriberRepository
{
    Task<Subscriber?> GetByEmailAsync(string email);
    Task<Subscriber?> GetByIdAsync(Guid id);
    Task<List<Subscriber>> GetAllActiveAsync();
    Task<Subscriber> CreateAsync(Subscriber subscriber);
    Task<bool> SetActiveAsync(Guid id, bool isActive);
}
