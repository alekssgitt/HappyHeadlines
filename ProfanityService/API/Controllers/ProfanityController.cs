using Microsoft.AspNetCore.Mvc;
using ProfanityService.Application.DTO;
using ProfanityService.Application.Interfaces;

namespace ProfanityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfanityController(IProfanityService service) : ControllerBase
{
    
    private const string CheckTextRoute = "check-text";
    private const string GetAllWordsRoute = "get-all-words";
    private const string AddWordRoute = "add-word";
    private const string DeleteWordRoute = "delete-word/{id:guid}";
    
    
    
    [HttpPost]
    [Route(CheckTextRoute)]
    public async Task<IActionResult> CheckText([FromBody] CheckTextDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await service.CheckTextAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    [Route(GetAllWordsRoute)]
    public async Task<IActionResult> GetAllWords()
    {
        var words = await service.GetAllWordsAsync();
        return Ok(words);
    }

    
    
    [HttpPost]
    [Route(AddWordRoute)]
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

    [HttpDelete]
    [Route(DeleteWordRoute)]
    public async Task<IActionResult> DeleteWord(Guid id)
    {
        var deleted = await service.DeleteWordAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
