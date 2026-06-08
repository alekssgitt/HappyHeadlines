using Microsoft.AspNetCore.Mvc;
using SubscriberService.Application.DTO;
using SubscriberService.Application.Interfaces.FeatureFlags;

namespace SubscriberService.API.Controllers;

[ApiController]
[Route("api/release-toggle")]
public class ReleaseToggleController(IReleaseToggleService releaseToggleService) : ControllerBase
{
    [HttpGet("subscriber-service")]
    public IActionResult GetSubscriberServiceToggle()
    {
        return Ok(new { enabled = releaseToggleService.IsSubscriberServiceEnabled() });
    }

    [HttpPut("subscriber-service")]
    public IActionResult SetSubscriberServiceToggle([FromBody] SetSubscriberServiceToggleDto dto)
    {
        var enabled = releaseToggleService.SetSubscriberServiceEnabled(dto.Enabled);
        return Ok(new { enabled });
    }
}
