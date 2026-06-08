using System.ComponentModel.DataAnnotations;

namespace SubscriberService.Application.DTO;

public class SubscribeDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}

public class SetSubscriberServiceToggleDto
{
    public bool Enabled { get; set; }
}
