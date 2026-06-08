using NewsletterService.Application.DTO;
using NewsletterService.Application.Interfaces;
using System.Net.Http.Json;

namespace NewsletterService.Infrastructure.External;

public class SubscriberClient(IHttpClientFactory httpClientFactory) : ISubscriberClient
{
    private const string SubscriberServiceClientName = "SubscriberServiceClient";
    private const string GetAllSubscribersRoute = "/api/subscribers/get-all-active";

    public async Task<List<SubscriberDto>> GetAllActiveSubscribersAsync()
    {
        var client = httpClientFactory.CreateClient(SubscriberServiceClientName);
        var result = await client.GetFromJsonAsync<List<SubscriberDto>>(GetAllSubscribersRoute);
        return result ?? [];
    }
}
