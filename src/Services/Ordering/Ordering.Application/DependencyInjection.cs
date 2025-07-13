
using System.Reflection;
using BuildingBlocks.Behaviours;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement; // From BB jer Application ga referencira

namespace Ordering.Application
{   /*Ovo sto pisem ovde, bih morao pisati u Program.cs, ali da tamo ne gomilam kod za svaki layer, zato pisem u ovoj klasi extension method za IServiceCollection 
    jer svaki "builder.Services.Add.." u Program.cs je IServiceCollection. */
    public static class DependencyInjection
    {   // Extension method (koji uvek mora biti static) for Application layer
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container 

            // Mediator je u Application layer iako sam ga koristio i u Infrastructure.
            services.AddMediatR(config =>
            {  
                config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
               // Register mediator Pipeline Behaviour (Validation and Logging) from BuildingBlocks
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
               // Kako bi se povezao sa CommandValidator klasom 
                config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                // Kako bi se povezao sa Command/QueryHandler klasom u Handler
            });

            /* Omogucava da tokom app running simuliram da li zelim da se OrderCreatedEventHandler aktivira ili ne. Treba da omogucim rucno, aktivaciju toga samo ako je BasketCheckout, a ako sam 
             * gadjao CreateOrderEndpoint onda mi to ne treba. Pogledaj OrderCreatedEvenHandler objasnjenje. */
            services.AddFeatureManagement();  

            // Ordering is Subscriber na RabbitMQ i zato AddMessageBroker(configuration, Assembly.GetExecutingAssembly()). Kod Basket ovo je samo AddMessageBroker(configuration).
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
