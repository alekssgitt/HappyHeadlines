using Microsoft.AspNetCore.Mvc;
using PublisherService.Application.DTO;
using PublisherService.Application.Interfaces;

namespace PublisherService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController(IPublisherService service) : ControllerBase
{
    private const string PublishArticleRoute = "publish-article";

    [HttpPost]
    [Route(PublishArticleRoute)]
    public async Task<IActionResult> PublishArticle([FromBody] PublishArticleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var articleId = await service.PublishAsync(dto);
        return Ok(new { articleId, status = "queued" });
    }
}
