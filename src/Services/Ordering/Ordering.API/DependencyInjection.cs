using BuildingBlocks.Exceptions;
using Carter;

namespace Ordering.API
{   /*Ovo sto pisem ovde, bih morao pisati u Program.cs, ali da tamo ne gomilam kod za svaki layer, zato pisem u ovoj klasi extension method za IServiceCollection 
    jer svaki "builder.Services.Add.." u Program.cs je IServiceCollection. */
    public static class DependencyInjection
    {
        // Extension method (koji uvek mora biti static) for API layer
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add services to container 
            
            // Carter mora u service, a ne u BuildingBLocks, jer Postman nece raditi ako nije tako
            services.AddCarter();// u Clean architecture u API layer je Endpoint klasa
            
            // Custom Exception Handler from BuildingBlocks da u Postman lepsa poruka bude mora u API jer Endpoint je API layer */
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Healt check za postman for SQL Server
            services.AddHealthChecks().AddSqlServer(configuration.GetConnectionString("Database")!);

            return services;
        }
        
        // Ovo je ono nakon builder.Build() iz Program.cs i ovo samo u API layer ima. Ostali layeri samo ovaj extension iznad imaju gde registruju servise.
        public static WebApplication UseApiServices(this WebApplication app)
        { 
            app.MapCarter(); 
            app.UseExceptionHandler(options => { });
            app.UseHealthChecks("health/");

            return app;
        }
    }
}
