using CommentService.Application.DTO;
using CommentService.Application.Interfaces;
using CommentService.Application.Interfaces.Data;
using CommentService.Application.Interfaces.External;
using CommentService.Domain;

namespace CommentService.Application.Services;

public class CommentService(
    ICommentRepository repository,
    IProfanityClient profanityClient,
    ILogger<CommentService> logger) : ICommentService
{
    public async Task<Comment> CreateAsync(CreateCommentDto dto)
    {
        var profanityResult = await profanityClient.CheckTextAsync(dto.Content);

        if (profanityResult.ContainsProfanity)
            logger.LogInformation("Profanity detected in comment, filtered {Count} words",
                profanityResult.DetectedWords.Count);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = dto.ArticleId,
            Author = dto.Author,
            Content = profanityResult.FilteredText,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(comment);
        logger.LogInformation("Comment {Id} created for article {ArticleId}", comment.Id, comment.ArticleId);

        return comment;
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<List<Comment>> GetByArticleIdAsync(Guid articleId)
    {
        return await repository.GetByArticleIdAsync(articleId);
    }

    public async Task<Comment?> UpdateAsync(Guid id, UpdateCommentDto dto)
    {
        string? filteredContent = null;

        if (dto.Content is not null)
        {
            var profanityResult = await profanityClient.CheckTextAsync(dto.Content);
            filteredContent = profanityResult.FilteredText;

            if (profanityResult.ContainsProfanity)
                logger.LogInformation("Profanity detected in comment update, filtered {Count} words",
                    profanityResult.DetectedWords.Count);
        }

        var updated = await repository.UpdateAsync(id, comment =>
        {
            if (filteredContent is not null) comment.Content = filteredContent;
        });

        if (updated is not null)
            logger.LogInformation("Comment {Id} updated", id);

        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);
        if (deleted)
            logger.LogInformation("Comment {Id} deleted", id);
        return deleted;
    }
}
