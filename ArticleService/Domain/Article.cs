using System.ComponentModel.DataAnnotations;

namespace ArticleService.Domain;

public class Article
{
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

