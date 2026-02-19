using ArticleService.Infrastructure;

namespace ArticleService.Application.Interfaces.Data;

public interface IDatabaseRouter
{
    ArticleDbContext GetDbContext(string continent);
    IEnumerable<ArticleDbContext> GetAllDbContexts();
    string[] ValidContinents { get; }
}
