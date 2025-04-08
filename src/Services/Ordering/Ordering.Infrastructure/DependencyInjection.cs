
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
    /*Ovo sto pisem ovde, bih morao pisati u Program.cs, ali da tamo ne gomilam kod 
   jer svaki layer ima  i zato pisem u ovoj klasi extension method za IServiceCollection 
   jer svaki "builder.Services.Add.." u Program.cs je za IServiceCollection. */
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration)
        { // Extension method for Infrastructure layer

            // Zbog IConfiguration argumenta, u Program.cs bice builder.Services.AddInfrastructureServices(builder.Configuration)
            
            // U Ordering.API appsettings (jer samo tamo postoji) definisacu ovo da bi se ocitalo ovde
            var connectionString = configuration.GetConnectionString("Database");

            // AddScoped jer imam vise Intercepotors (AuditableEntityInterceptor.cs i DispatchDomainEventInterceptor.cs) 
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();

            // Add services to the container 
            services.AddDbContext<ApplicationDbContext>(config =>
            {   /* Register Interceptor for classes that implemented IEntity (Customer.cs, Order.cs, OrderItem.cs, Product.cs)
                tj za tabele Orders, OrderItems, Customers i Products, da upise vrednosti u Audit kolone ako se modifikuje tabela. */
                config.AddInterceptors(new AuditableEntityInterceptor());
                config.UseSqlServer(connectionString);
            });
                
            // Registrujem da ApplicationDbContext prepoznaje se kao IApplicationDbContext
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            return services;    
        }
    }
}
