using ArticleService.Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Infrastructure
{
    public class DatabaseRouter(IConfiguration configuration) : IDatabaseRouter
    {

        public string[] ValidContinents { get; } =
        [
            "africa",
            "antarctica",
            "asia",
            "europe",
            "north-america",
            "south-america",
            "oceania",
            "global"
        ];

        public ArticleDbContext GetDbContext(string continent)
        {
            var normalised = continent.ToLowerInvariant().Trim();

            if (!ValidContinents.Contains(normalised))
            {
                throw new ArgumentException(
                    $"Invalid continent '{continent}'. Valid values: {string.Join(", ", ValidContinents)}");
            }

            var connectionString = configuration.GetConnectionString(GetConnectionKey(normalised));

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"No connection string found for continent '{normalised}'.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ArticleDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ArticleDbContext(optionsBuilder.Options);
        }

        public IEnumerable<ArticleDbContext> GetAllDbContexts()
        {
            foreach (var continent in ValidContinents)
            {
                yield return GetDbContext(continent);
            }
        }

        private static string GetConnectionKey(string continent)
        {
            return continent switch
            {
                "africa" => "ArticleDb_Africa",
                "antarctica" => "ArticleDb_Antarctica",
                "asia" => "ArticleDb_Asia",
                "europe" => "ArticleDb_Europe",
                "north-america" => "ArticleDb_NorthAmerica",
                "south-america" => "ArticleDb_SouthAmerica",
                "oceania" => "ArticleDb_Oceania",
                "global" => "ArticleDb_Global",
                _ => throw new ArgumentException($"Unknown continent: {continent}")
            };
        }
    }
}
