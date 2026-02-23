using CommentService.Application.Interfaces.Data;
using CommentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure.Repositories;

public class CommentRepository(CommentDbContext context) : ICommentRepository
{
    public async Task<Comment> CreateAsync(Comment comment)
    {
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Comment>> GetByArticleIdAsync(Guid articleId)
    {
        return await context.Comments
            .AsNoTracking()
            .Where(c => c.ArticleId == articleId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment?> UpdateAsync(Guid id, Action<Comment> applyUpdates)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return null;

        applyUpdates(comment);
        comment.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return false;

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        return true;
    }
}
