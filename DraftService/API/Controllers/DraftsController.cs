using DraftService.Application.DTO;
using DraftService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DraftService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftsController(IDraftService service) : ControllerBase
{
    private const string CreateDraftRoute = "create-draft";
    private const string GetDraftByIdRoute = "get-draft-by-id/{id:guid}";
    private const string GetAllDraftsRoute = "get-all-drafts";
    private const string UpdateDraftRoute = "update-draft/{id:guid}";
    private const string DeleteDraftRoute = "delete-draft/{id:guid}";

    [HttpPost]
    [Route(CreateDraftRoute)]
    public async Task<IActionResult> Create([FromBody] CreateDraftDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var draft = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = draft.Id }, draft);
    }

    [HttpGet]
    [Route(GetDraftByIdRoute)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var draft = await service.GetByIdAsync(id);
        return draft is null ? NotFound() : Ok(draft);
    }

    [HttpGet]
    [Route(GetAllDraftsRoute)]
    public async Task<IActionResult> GetAll()
    {
        var drafts = await service.GetAllAsync();
        return Ok(drafts);
    }

    [HttpPut]
    [Route(UpdateDraftRoute)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDraftDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete]
    [Route(DeleteDraftRoute)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
