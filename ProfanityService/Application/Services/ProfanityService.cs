using System.Text.RegularExpressions;
using ProfanityService.Application.DTO;
using ProfanityService.Application.Interfaces;
using ProfanityService.Application.Interfaces.Data;
using ProfanityService.Domain;

namespace ProfanityService.Application.Services;

public class ProfanityService(IProfanityRepository repository, ILogger<ProfanityService> logger) : IProfanityService
{
    public async Task<CheckTextResultDto> CheckTextAsync(CheckTextDto dto)
    {
        var words = await repository.GetAllAsync();
        var detectedWords = new List<string>();
        var filteredText = dto.Text;

        foreach (var profanityWord in words)
        {
            var pattern = $@"\b{Regex.Escape(profanityWord.Word)}\b";
            if (Regex.IsMatch(filteredText, pattern, RegexOptions.IgnoreCase))
            {
                detectedWords.Add(profanityWord.Word);
                var replacement = new string('*', profanityWord.Word.Length);
                filteredText = Regex.Replace(filteredText, pattern, replacement, RegexOptions.IgnoreCase);
            }
        }

        logger.LogInformation("Profanity check: {Detected} words found", detectedWords.Count);

        return new CheckTextResultDto
        {
            OriginalText = dto.Text,
            FilteredText = filteredText,
            ContainsProfanity = detectedWords.Count > 0,
            DetectedWords = detectedWords
        };
    }

    public async Task<List<ProfanityWord>> GetAllWordsAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<ProfanityWord> AddWordAsync(AddWordDto dto)
    {
        var existing = await repository.GetByWordAsync(dto.Word);
        if (existing is not null)
            throw new InvalidOperationException($"Word '{dto.Word}' already exists");

        var profanityWord = new ProfanityWord
        {
            Id = Guid.NewGuid(),
            Word = dto.Word.ToLowerInvariant().Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(profanityWord);
        logger.LogInformation("Profanity word added: {Word}", profanityWord.Word);

        return profanityWord;
    }

    public async Task<bool> DeleteWordAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);
        if (deleted)
            logger.LogInformation("Profanity word {Id} deleted", id);
        return deleted;
    }
}
