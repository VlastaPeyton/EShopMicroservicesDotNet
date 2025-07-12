
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Data.Extensions
{
    // Obzirom da 4 tabele moram da seedujem i napravim initial data, to je mnogo koda za OnModelCreating, pa zato ovde stavljam Seeding logiku, dok intial data stavim u InitialData.cs

    // Kod Discount isto sam imao Auto Migration, jer ocu auto migration prilikom app startup jer tad ne zelim Package Manager Console da otvorim da kucam Add-Migration Update-Database
    public static class DatabaseExtensions
    {   // Extension method (uvek mora static) 
        public static async Task InitializeDatabaseAsync(this WebApplication app) 
        {
            // Auto migrate database 
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            // context.Database = OrderingDb 
            // MigrateAsync().GetAwaiter().GetResult() ako ocemo da od MigrateAsync() napravimo sync ali ne treba nam 

            // Seeding database 
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
                await context.SaveChangesAsync(); 
            }
        }

        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            // Da li ima nesto u Products tabeli, jer ako ima, ne Seedujemo
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(InitialData.ProductsInitialData);
                await context.SaveChangesAsync(); 
            }
        }

        public static async Task SeedOrderAndItemsAsync(ApplicationDbContext context)
        {
            // Da li ima nesto u Orders tabeli, jer ako ima, ne Seedujemo
            if (!await context.Products.AnyAsync())
            {
                await context.Orders.AddRangeAsync(InitialData.OrderWithOrderItemsInitialData);
                await context.SaveChangesAsync(); 
            }
        }
    }
}
