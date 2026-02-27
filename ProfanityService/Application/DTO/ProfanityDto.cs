using System.ComponentModel.DataAnnotations;

namespace ProfanityService.Application.DTO;

public class AddWordDto
{
    [Required]
    [MaxLength(100)]
    public string Word { get; set; } = string.Empty;
}

public class CheckTextDto
{
    [Required]
    public string Text { get; set; } = string.Empty;
}

public class CheckTextResultDto
{
    public string OriginalText { get; set; } = string.Empty;
    public string FilteredText { get; set; } = string.Empty;
    public bool ContainsProfanity { get; set; }
    public List<string> DetectedWords { get; set; } = [];
}
