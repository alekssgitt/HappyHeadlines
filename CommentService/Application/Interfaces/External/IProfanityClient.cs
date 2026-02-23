namespace CommentService.Application.Interfaces.External;

public class ProfanityCheckResult
{
    public string OriginalText { get; set; } = string.Empty;
    public string FilteredText { get; set; } = string.Empty;
    public bool ContainsProfanity { get; set; }
    public List<string> DetectedWords { get; set; } = [];
}

public interface IProfanityClient
{
    Task<ProfanityCheckResult> CheckTextAsync(string text);
}
