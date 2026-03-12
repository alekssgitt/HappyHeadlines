using DraftService.Application.DTO;
using DraftService.Application.Interfaces;
using DraftService.Application.Interfaces.Data;
using DraftService.Domain;

namespace DraftService.Application.Services;

public class DraftService(IDraftRepository repository, ILogger<DraftService> logger) : IDraftService
{
    public async Task<Draft> CreateAsync(CreateDraftDto dto)
    {
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            Author = dto.Author,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(draft);
        logger.LogInformation("Draft {DraftId} created for author {Author}", draft.Id, draft.Author);
        return draft;
    }

    public async Task<Draft?> GetByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<List<Draft>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Draft?> UpdateAsync(Guid id, UpdateDraftDto dto)
    {
        var updated = await repository.UpdateAsync(id, draft =>
        {
            if (dto.Title is not null) draft.Title = dto.Title;
            if (dto.Content is not null) draft.Content = dto.Content;
            if (dto.Author is not null) draft.Author = dto.Author;
            if (dto.Status is not null) draft.Status = dto.Status;
        });

        if (updated is not null)
            logger.LogInformation("Draft {DraftId} updated", id);

        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);
        if (deleted)
            logger.LogInformation("Draft {DraftId} deleted", id);
        return deleted;
    }
}
