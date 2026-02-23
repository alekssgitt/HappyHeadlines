using CommentService.Application.Interfaces.External;

namespace CommentService.Infrastructure.External;

public class ProfanityClientFallback(IProfanityClient inner, ILogger<ProfanityClientFallback> logger) : IProfanityClient
{
    public async Task<ProfanityCheckResult> CheckTextAsync(string text)
    {
        try
        {
            return await inner.CheckTextAsync(text);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "ProfanityService unavailable — circuit breaker fallback: allowing comment through unfiltered");

            return new ProfanityCheckResult
            {
                OriginalText = text,
                FilteredText = text,
                ContainsProfanity = false,
                DetectedWords = []
            };
        }
    }
}
