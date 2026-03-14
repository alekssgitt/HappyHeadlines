using System.ComponentModel.DataAnnotations;

namespace PublisherService.Application.DTO;

public class PublishArticleDto
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Continent { get; set; } = string.Empty;
}
