using CommentService.Application.DTO;
using CommentService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var comment = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var comment = await service.GetByIdAsync(id);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpGet("article/{articleId:guid}")]
    public async Task<IActionResult> GetByArticleId(Guid articleId)
    {
        var comments = await service.GetByArticleIdAsync(articleId);
        return Ok(comments);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
