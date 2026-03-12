using System.ComponentModel.DataAnnotations;

namespace DraftService.Application.DTO;

public class CreateDraftDto
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;
}

public class UpdateDraftDto
{
    [MaxLength(300)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [MaxLength(150)]
    public string? Author { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }
}
