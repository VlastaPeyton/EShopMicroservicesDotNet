

using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.MassTransit
{
    // Extension method for setting up MassTransit with RabbitMQ
    public static class Extensions
    {
        // As i wanna register AddMessageBroker into app startup in Program.cs in Basket i Ordering 
        public static IServiceCollection AddMessageBroker
            (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {   /*Assembly = null, jer za Publisher (Basket.API Program.cs) ne treba nam, ali za Consumer (Ordering.API Program.cs) 
             treba assembly != null. 
            */

            // Set up MassTransit in IServiceCollection 
            services.AddMassTransit(config =>
            {
                /*Set up naming convetion for Endpoints. KebabCase is used in URL (npr background-color), jer ne podrzava
                 space kao razmak u URL, vec samo crtica. */
                config.SetKebabCaseEndpointNameFormatter();

                // Ordering je Consumer i za njega mora assembly != null
                if (assembly != null)
                    config.AddConsumers(assembly);

                // Configure bus for using RabbitMQ
                config.UsingRabbitMq((context, configurator) =>
                {
                    // configuration dohvata zeljeno polje iz appsettings of current assebmly (Basket ili Ordering)
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
