using Microsoft.AspNetCore.Mvc;
using NewsletterService.Application.Interfaces;

namespace NewsletterService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController(INewsletterService service) : ControllerBase
{
    private const string GetDailyPreviewRoute = "daily-preview";
    private const string GetLatestQueuedRoute = "latest-queued";
    
    [HttpGet]
    [Route(GetDailyPreviewRoute)]
    public async Task<IActionResult> DailyPreview([FromQuery] int count = 5)
    {
        var articles = await service.GetDailyArticlesAsync(count);
        return Ok(articles);
    }

    [HttpGet]
    [Route(GetLatestQueuedRoute)]
    public IActionResult LatestQueued([FromQuery] int count = 5)
    {
        var articles = service.GetLatestQueuedArticles(count);
        return Ok(articles);
    }
}
