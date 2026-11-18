using GymManagment.DAL.Data.DataSeeding;
using GymManagment.DAL.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.PL
{
    public static class ProgramExtensions
    {
        public static async Task MigrationAndSeedingData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigrations =await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()}");
                await dbContext.Database.MigrateAsync();
            }

            var contentRootPath = app.Environment.ContentRootPath;

            var dataFolderPath = Path.Combine(contentRootPath, "WWWRoot", "Files");
            await GymDataSeeding.SeedDataAsync(dataFolderPath, dbContext, logger);
        }
    }
}
