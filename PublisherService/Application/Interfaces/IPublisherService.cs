using PublisherService.Application.DTO;

namespace PublisherService.Application.Interfaces;

public interface IPublisherService
{
    Task<Guid> PublishAsync(PublishArticleDto dto);
}
