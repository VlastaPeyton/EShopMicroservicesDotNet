
using System.Reflection;
using BuildingBlocks.Behaviours;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement; // From BB jer Application ga referencira

namespace Ordering.Application
{   /*Ovo sto pisem ovde, bih morao pisati u Program.cs, ali da tamo ne gomilam kod 
    jer svaki layer ima  i zato pisem u ovoj klasi extension method za IServiceCollection 
    jer svaki "builder.Services.Add.." u Program.cs je za IServiceCollection. */
    public static class DependencyInjection
    {   // Extension method for Application layer
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container 

            // Mediator je u Application layer
            services.AddMediatR(config =>
            {  // Jer DispatchDomainEventsInterceptor koristi MediatR kao DI i
                config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
               // Register mediator Pipeline Behaviour (Validation and Logging) from BuildingBlocks
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
               // Kako bi se povezao sa CommandValidator klasom 
                config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                // Kako bi se povezao sa Command/QueryHandler klasom u Handler
            });

            /* Add Feature Management da izbegnem publish OrderCreatedIntegrationEvent from OrderCreatedEventHandler  za slucajeve 
            kad nisam na frontendu (Client) kliknuo Checkout, vec kad sam startovao app (seedovao Ordering baze ili iz Postman gadjao 
            CreateOrderEndpoint */
            services.AddFeatureManagement();

            /* Kao u Basket Program.cs sto sam dodao AddMessageBroker iz BuildingBlocks.Messaging, samo sto ovde
            AddMessageBroker(configuration, Assembly.GetExecutingAssembly()), jer Ordering je Subscriber na RabbitMQ
            za Integration Event. Zbog ovoga, morao sam da AddApplicationServices extension method ovaj prosiri da prima 
            IConfiguration configuration kao argument, a onda u API layer Program.cs morace AddApplicationServices(builder.Configuration)*/
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
