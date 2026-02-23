using System.ComponentModel.DataAnnotations;

namespace CommentService.Application.DTO;

public class CreateCommentDto
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}

public class UpdateCommentDto
{
    public string? Content { get; set; }
}
