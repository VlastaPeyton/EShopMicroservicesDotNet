using System.Runtime.CompilerServices;
using Discount.gRPC.Data;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Extensions
{
    public static class Extensions
    {
        // To auto migrate SQL DB
        public static async Task<IApplicationBuilder> UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope(); ;
            using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();
            // Auto Migrate
            await dbContext.Database.MigrateAsync();
            return app;
        }
    }
}
