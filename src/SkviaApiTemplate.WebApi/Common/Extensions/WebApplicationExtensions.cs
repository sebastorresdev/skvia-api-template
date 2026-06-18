using SkviaApiTemplate.WebApi.Persistence.Data;

namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task<WebApplication> InitializeDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();
            await AppDbContextSeed.SeedAsync(db);

            return app;
        }
    }
}