using System.Net.Http.Json;
using CommentService.Application.Interfaces.External;

namespace CommentService.Infrastructure.External;

public class ProfanityClient(HttpClient httpClient, ILogger<ProfanityClient> logger) : IProfanityClient
{
    public async Task<ProfanityCheckResult> CheckTextAsync(string text)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/profanity/check", new { text });
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProfanityCheckResult>();

            return result ?? new ProfanityCheckResult
            {
                OriginalText = text,
                FilteredText = text,
                ContainsProfanity = false,
                DetectedWords = []
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ProfanityService call failed — circuit breaker fallback activated");
            throw;
        }
    }
}
