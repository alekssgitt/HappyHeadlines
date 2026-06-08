using Microsoft.AspNetCore.Mvc;
using SubscriberService.Application.DTO;
using SubscriberService.Application.Interfaces;
using SubscriberService.Application.Interfaces.FeatureFlags;

namespace SubscriberService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscribersController(
    ISubscriberService service,
    IReleaseToggleService releaseToggleService) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeDto dto)
    {
        if (!releaseToggleService.IsSubscriberServiceEnabled())
            return StatusCode(503, new { message = "SubscriberService is currently disabled by release toggle." });

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var subscriber = await service.SubscribeAsync(dto);
        return Ok(subscriber);
    }

    [HttpPut("unsubscribe/{id:guid}")]
    public async Task<IActionResult> Unsubscribe(Guid id)
    {
        if (!releaseToggleService.IsSubscriberServiceEnabled())
            return StatusCode(503, new { message = "SubscriberService is currently disabled by release toggle." });

        var success = await service.UnsubscribeAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("get-all-active")]
    public async Task<IActionResult> GetAllActive()
    {
        if (!releaseToggleService.IsSubscriberServiceEnabled())
            return StatusCode(503, new { message = "SubscriberService is currently disabled by release toggle." });

        var subscribers = await service.GetAllActiveAsync();
        return Ok(subscribers);
    }
}
