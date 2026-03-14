namespace NewsletterService.Application.DTO;

public class ArticleSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Continent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
