
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Data;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Data.Interceptors;
// Ova 2 su iz BB jer ga Infrastructure referencira. 
namespace Ordering.Infrastructure
{
    /*Ovo sto pisem ovde, bih morao pisati u Program.cs, ali da tamo ne gomilam kod za svaki layer, zato pisem u ovoj klasi extension method za IServiceCollection 
     jer svaki "builder.Services.Add.." u Program.cs je IServiceCollection. */
    public static class DependencyInjection
    {   // Extension method (koji je uvek static) za Infrastructure layer
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        { 
            // Zbog IConfiguration argumenta, u Program.cs bice builder.Services.AddInfrastructureServices(builder.Configuration)
            
            // U Ordering.API appsettings (jer samo tamo postoji appsettings jer je API layer ASP.NET Core Empty projekat) definisacu Connection String da bih mogo da ga ocitam ovde
            var connectionString = configuration.GetConnectionString("Database");

            // AddScoped jer sam ApplicationDbContext registrovao kao AddScoped, a Interceptors se odnose na DbContext jer on reaguje sa EF Core.
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();

            // Add services to the container 
            services.AddDbContext<ApplicationDbContext>(config =>
            {  
                config.UseSqlServer(connectionString);
            });

            // Registrujem da ApplicationDbContext prepoznaje se kao IApplicationDbContext
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            return services;    
        }
    }
}
