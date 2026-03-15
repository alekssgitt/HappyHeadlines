using CommentService.Application.DTO;
using CommentService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService service) : ControllerBase
{
    
    private const string CreateCommentRoute = "create-comment";
    private const string GetCommentByIdRoute = "get-comment-by-id/{id:guid}";
    private const string GetCommentByArticleIdRoute = "get-comment-by-article-id/{articleId:guid}";
    private const string UpdateCommentRoute = "update-comment/{id:guid}";
    private const string DeleteCommentRoute = "delete-comment/{id:guid}";
    
    
    
    [HttpPost]
    [Route(CreateCommentRoute)]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var comment = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [HttpGet]
    [Route(GetCommentByIdRoute)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var comment = await service.GetByIdAsync(id);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpGet]
    [Route(GetCommentByArticleIdRoute)]
    public async Task<IActionResult> GetByArticleId(Guid articleId)
    {
        var comments = await service.GetByArticleIdAsync(articleId);
        return Ok(comments);
    }

    [HttpPut]
    [Route(UpdateCommentRoute)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete]
    [Route(DeleteCommentRoute)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
