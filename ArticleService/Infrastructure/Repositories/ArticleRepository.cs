using ArticleService.Application.Interfaces.Data;
using ArticleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Infrastructure.Repositories;

public class ArticleRepository(IDatabaseRouter dbRouter) : IArticleRepository
{

    public async Task<Article> CreateAsync(Article article, string continent)
    {
        using var db = dbRouter.GetDbContext(continent);
        db.Articles.Add(article);
        await db.SaveChangesAsync();
        return article;
    }

    public async Task<Article?> GetByIdAsync(Guid id, string? continent)
    {
        if (!string.IsNullOrWhiteSpace(continent))
        {
            using var db = dbRouter.GetDbContext(continent);
            return await db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }
        
        foreach (var db in dbRouter.GetAllDbContexts())
        {
            using (db)
            {
                var article = await db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                if (article is not null)
                    return article;
            }
        }

        return null;
    }

    public async Task<List<Article>> GetAllAsync(string? continent)
    {
        if (!string.IsNullOrWhiteSpace(continent))
        {
            using var db = dbRouter.GetDbContext(continent);
            return await db.Articles.AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
        
        var results = new List<Article>();
        foreach (var db in dbRouter.GetAllDbContexts())
        {
            using (db)
            {
                var articles = await db.Articles.AsNoTracking()
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
                results.AddRange(articles);
            }
        }

        return results.OrderByDescending(a => a.CreatedAt).ToList();
    }

    public async Task<Article?> UpdateAsync(Guid id, Action<Article> applyUpdates, string? continent)
    {
        async Task<Article?> TryUpdate(ArticleDbContext db)
        {
            var article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article is null) return null;

            applyUpdates(article);
            article.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return article;
        }

        if (!string.IsNullOrWhiteSpace(continent))
        {
            using var db = dbRouter.GetDbContext(continent);
            return await TryUpdate(db);
        }

        foreach (var db in dbRouter.GetAllDbContexts())
        {
            using (db)
            {
                var updated = await TryUpdate(db);
                if (updated is not null) return updated;
            }
        }

        return null;
    }

    public async Task<bool> DeleteAsync(Guid id, string? continent)
    {
        async Task<bool> TryDelete(ArticleDbContext db)
        {
            var article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article is null) return false;

            db.Articles.Remove(article);
            await db.SaveChangesAsync();
            return true;
        }

        if (!string.IsNullOrWhiteSpace(continent))
        {
            using var db = dbRouter.GetDbContext(continent);
            return await TryDelete(db);
        }

        foreach (var db in dbRouter.GetAllDbContexts())
        {
            using (db)
            {
                if (await TryDelete(db)) return true;
            }
        }

        return false;
    }
}
