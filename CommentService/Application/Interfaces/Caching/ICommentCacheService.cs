using CommentService.Domain;

namespace CommentService.Application.Interfaces.Caching;

public interface ICommentCacheService
{
    Task<List<Comment>?> GetCommentsByArticleIdAsync(Guid articleId);
    Task SetCommentsByArticleIdAsync(Guid articleId, List<Comment> comments);
    Task InvalidateArticleCommentsAsync(Guid articleId);

    void RecordCommentCacheHit();
    void RecordCommentCacheMiss();
}
