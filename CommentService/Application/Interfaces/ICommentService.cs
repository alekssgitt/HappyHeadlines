using CommentService.Application.DTO;
using CommentService.Domain;

namespace CommentService.Application.Interfaces;

public interface ICommentService
{
    Task<Comment> CreateAsync(CreateCommentDto dto);
    Task<Comment?> GetByIdAsync(Guid id);
    Task<List<Comment>> GetByArticleIdAsync(Guid articleId);
    Task<Comment?> UpdateAsync(Guid id, UpdateCommentDto dto);
    Task<bool> DeleteAsync(Guid id);
}
