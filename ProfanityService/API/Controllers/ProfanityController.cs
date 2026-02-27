using Microsoft.AspNetCore.Mvc;
using ProfanityService.Application.DTO;
using ProfanityService.Application.Interfaces;

namespace ProfanityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfanityController(IProfanityService service) : ControllerBase
{
    [HttpPost("check")]
    public async Task<IActionResult> CheckText([FromBody] CheckTextDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await service.CheckTextAsync(dto);
        return Ok(result);
    }

    [HttpGet("words")]
    public async Task<IActionResult> GetAllWords()
    {
        var words = await service.GetAllWordsAsync();
        return Ok(words);
    }

    [HttpPost("words")]
    public async Task<IActionResult> AddWord([FromBody] AddWordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var word = await service.AddWordAsync(dto);
            return Created($"/api/profanity/words/{word.Id}", word);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("words/{id:guid}")]
    public async Task<IActionResult> DeleteWord(Guid id)
    {
        var deleted = await service.DeleteWordAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
