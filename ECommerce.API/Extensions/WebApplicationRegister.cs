using ECommerce.Domain.Contracts;
using ECommerce.Presistence.Data.DbContexts;
using ECommerce.Presistence.IdentityData.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Extensions
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDataBaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbConetxt>();
            var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (PendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }

            return app;
        }
        public static async Task<WebApplication> MigrateIdentityDataBaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StoreIdentityDbContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }

            return app;
        }
        public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>(
                "Default");
            await dataInitializer.InitializeAsync();
            return app;
        }
        public static async Task<WebApplication> SeedIdentityDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dataIntializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>(
                "Identity");

            await dataIntializer.InitializeAsync();

            return app;
        }
    }
}
