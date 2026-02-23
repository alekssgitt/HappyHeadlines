using CommentService.Domain;

namespace CommentService.Application.Interfaces.Data;

public interface ICommentRepository
{
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment?> GetByIdAsync(Guid id);
    Task<List<Comment>> GetByArticleIdAsync(Guid articleId);
    Task<Comment?> UpdateAsync(Guid id, Action<Comment> applyUpdates);
    Task<bool> DeleteAsync(Guid id);
}
