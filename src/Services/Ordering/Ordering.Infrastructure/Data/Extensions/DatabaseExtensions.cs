
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Data.Extensions
{
    // Logika za EF migracije
    public static class DatabaseExtensions
    {   // Extension method jer ocu migraciju automatksi prilikom app startup
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            // Auto migrate database 
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();//.GetAwaiter().GetResult(); 
            // context.Database = OrderingDb 
            // MigrateAsync().GetAwaiter().GetResult() ako ocemo da od MigrateAsync() napravimo sync ali msm da ne treba nam 

            // Seeding databasem 
            await SeedAsync(context);
        }

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedCustomersAsync(context);
            await SeedProductsAsync(context);
            await SeedOrderAndItemsAsync(context);
        }

        public static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            // Da li ima nesto u Customers tabeli, jer ako ima, ne Seedujemo 
            if (!await context.Customers.AnyAsync()) // AnyAsync je built-in
            {
                await context.Customers.AddRangeAsync(InitialData.CustomersInitialData); // AddRangeAsync je built-in
                await context.SaveChangesAsync(); // Sadrzi commit/rollback u sebi
            }
        }

        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            // Da li ima nesto u Products tabeli, jer ako ima, ne Seedujemo
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(InitialData.ProductsInitialData);
                await context.SaveChangesAsync(); // Sadrzi commit/rollback u sebi
            }
        }

        public static async Task SeedOrderAndItemsAsync(ApplicationDbContext context)
        {
            // Da li ima nesto u Orders tabeli, jer ako ima, ne Seedujemo
            if (!await context.Products.AnyAsync())
            {
                await context.Orders.AddRangeAsync(InitialData.OrderWithOrderItemsInitialData);
                await context.SaveChangesAsync(); // Sadrzi commit/rollback u sebi
            }
        }
    }
}
