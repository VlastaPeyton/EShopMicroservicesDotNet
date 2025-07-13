using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.MassTransit
{
    // Extension method (uvek static) for setting up MassTransit with RabbitMQ, pa da ne kucam ceo ovaj kod u Program.cs of Ordering i Basket
    public static class Extensions
    {   
        // Ovu metodu pozivam u Program.cs of Basket i u DependencyInjection.cs (Program.cs) of Ordering.Application, jer Basket i Ordering koriste RabbitMQ
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {   
            // Kada je pozivam u Basket, bice "builder.Services.AddMessageBroker(builder.Configuration)" jer assembly tad mora = null
            // Kada je pozivam u Ordering, bice "builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly())" jer assembly tad mora != null

            // Set up MassTransit library  
            services.AddMassTransit(config =>
            {
                // Set up naming convetion for Endpoints. KebabCase is used in URL (npr background-color), jer ne podrzava space kao razmak u URL, vec samo crtica.
                config.SetKebabCaseEndpointNameFormatter();

                // Ordering je Consumer i za njega mora assembly != null tj u Ordering prosledjujem i configuration i assembly argument
                if (assembly is not null)
                    config.AddConsumers(assembly);

                // Configure MassTransit message bus for using RabbitMQ
                config.UsingRabbitMq((context, configurator) =>
                {
                    // configuration dohvata zeljeno polje iz appsettings of current assebmly (Basket ili Ordering) jer oba servisa imaju MessageBroker definisan u appsettings.
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]);
                        host.Password(configuration["MessageBroker:Password"]);
                    });
                    // MassTransit automatski configures Endpoints for Consumer(Subscriber) tj za Ordering
                    configurator.ConfigureEndpoints(context);
                });
            });

            // Implement RabbitMQ MassTransit configuration nakon sto definisem RabbitMQ
            return services;
        }
    }
}
