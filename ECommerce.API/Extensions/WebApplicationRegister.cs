using ECommerce.Domain.Contracts;
using ECommerce.Presistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ECommerce.API.Extensions
{
    public static class WebApplicationRegister
    {
        public static  async Task<WebApplication> MigrateDataBaseAsync(this WebApplication app)
        {
            await using var scope =  app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbConetxt>();
            var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (PendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }

            return app;
        }
        public static  async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope(); 
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            await dataInitializer.InitializeAsync();
            return app;
        }
    }
}
