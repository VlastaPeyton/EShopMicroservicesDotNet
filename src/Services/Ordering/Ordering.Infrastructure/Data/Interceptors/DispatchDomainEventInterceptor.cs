

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.Domain.Abstractions;

namespace Ordering.Infrastructure.Data.Interceptors
{   
    // Dispatch = Publish Domain Events in Ordering microservice
    public class DispatchDomainEventInterceptor(IMediator mediator) : SaveChangesInterceptor
    {   /* u DepdencyInjection.cs (Program.cs) mora da se registruje  da DispatchDomainEventInterceptor se odnosi na ISaveChangesInterceptor. 
        
           IMediator je pandan IPublishEndpoint u CheckoutBasketCommandHandler u Basket koji publish BasketCheckoutEvent
        (Integration event) to RabbitMQ na kog je Ordering service Subscribed. */

        // Kucam public override i ponudi nam metode i izaberemo ove 2 override
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            /* Extract Domain Events from Aggregate (Orrder.cs) and dispatch them, jer u Aggregate imamo OrderCreatedEvent i OrderUpdatedEvent
              
               GetAwaiter().GetResult() pravi sync from async, jer DispatchDomainEvents je async, dok SavingChanges mora biti sync, 
            pa zato morali da prebacimo from async to sync. 
               
               eventData.Context = ApplicationDbContext*/
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult(); // Nema await, jer sam napravio sync from async
            return base.SavingChanges(eventData, result);
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
        {
            // SavingChangesAsync je asyn i zato nema GetAwaiter().GetResult() kao iznad jer DispathcDomainEvents je async 
            await DispatchDomainEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public async Task DispatchDomainEvents(DbContext? context)
        {   // ApplicationDbContext nasledila DbContext a znam da uvek roditelja stavljam zbog DDD
            if (context is null)
                return;

            /* Koristim IDomainEvent, a ne DomainEvent, u skladu sa DDD. 
               Trazim DbContext entitete koji implementiraju IAggregate, a to je Order.cs tj Orders tabela.
               DomainEvents je polje u IAggregate.cs
               ChangeTracker.Entries je svaka vrsta tabele.*/

            // Retrieve  enttites of type IAggregate (Order.cs) that have DomainEvents associated with them */
            var aggregates = context.ChangeTracker.Entries<IAggregate>() // Sve tabele (vrste) koje su nasledile IAggregate.
                                                 .Where(a => a.Entity.DomainEvents.Any()) // Entity je Order, jer samo on nasledio IAggregate. DomainEvents je lista u Order.cs i proveravam da l prazna
                                                 .Select(a => a.Entity); // Selektujem Order jer samo njega moze.

            // Retrieve DomainEvents from aggregates (ali samo Order je aggregate). SelectMany ako ima vise aggregata,a moze i za jednog.
            var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

            /* Avoid duplicate Dispatch i zato moram koristim ClearDomainEvents (metodu koja je potpisana u IAggregate)
            Kada smo izvukli u varijablu sve DomainEvents onda praznimo DomainEvents listu za svaki aggregate. 
            Da ClearDomainEvents nije bila potpisana u IAggregate, vec samo u Order, koriscenje nje ne bi moglo ovde. Plus ako imam vise agregata, onda takodje bez ovoga ne bi moglo jer zajedicna stvar za agregate mora biti u interface kojeg implementiraju */
            aggregates.ToList().ForEach(a => a.ClearDomainEvents()); // Mora ToList() jer to nije uradjeno u liniji na pocetku gde sam skupio aggregate

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent); // Aktivira se OrdredCreatedEvenHandler 


            /* Postoje 2 vrste Events:
                1) Domain Event  - Domain Event je Published(Dispatched) i Consumed(Subscribed) unutar istog service (Ordering u mom slucaju) i zato koristimo MediatR jer je unutar istog servisa. 
                                   Domain Event se Publish kad se desi modifikacija u bazi (moze i za reading from db, ali je retko).
                                   Za Domain Event kazem Dispatch(Publish)/Consume, a za Integration Event samo Publish/Consume.
                                   Imamo OrderCreatedEvent i OrderUpdatedEvent kao Domain Events def u Domain layer.

                2) Integration Event - Integration Event je Published from Service1 (Basket u mom slucaju) i Consumed in Service2 (Ordering u mom slucaju). Publish-Consume(Subscribe) se radi preko Message Broker (RabbitMQ + MassTransit library). 
                                       
              Domain Event flow:
                  IDomainEvent : Inotification iz MediatR.
                  OrderCreatedEvent/OrderUpdatedEvent : IDomainEvent.
                  OrderCreatedEventHandler/OrderUpdatedEventHandler : INotificationHandler<OrderCreatedEvent/OrderUpdatedEvent>/ i zbog ovoga, MediatR znace koji EventHandler da pozove kad mediator.Publish(domainEvent)
                  => mediator.Publish(domainEvent),a domainEvent moze biti OrderCreatedEvent/OrderUpdatedEvent => MediatR na osnovu tipa DomainEvent poziva odgovarajuci EventHandler. 

              Integration Event flow: 
                U Basket, CheckoutBasketCommandHandler ce da publishEndpoint.Publish(BasketCheckoutEvent) u RabbitMQ kada Client u frontend ide na checkout.
            Ordering je Subscriber za to na RabbitMQ i zbog BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>  ce u BasketCheckoutEventHandler da 
            se aktivira Consume metoda  koja ce da preko MediatR da prosledi CreateOrderCommand u  CreateOrderCommanHandler, a CreateOrderCommanHandler
            pokrece CreateNewOrder metodu koja pokrece Order.Create(iz Order.cs), a  Order.Create metoda ce pozvati AddDomainEvent da doda OrderCreatedEvent 
            u DomainEvents polje (koje Order nasledio iz Aggregate), a posto OrderCreatedEvent implements IDomainEvent (:INotification), automatski ce
            DispatchDomainEventsInterceptor (zbog IMediator) ce da uradi publish(OrderCreatedEvent), a OrderCreatedEventHandler aktivira se 
            (zbog OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>) pa ce da Handle OrderCreatedEvent tj da Publish 
            OrderCreatedIntegrationEvent (definisacu kasnije) u RabbitMQ (prvi put da Ordering je Publisher u RabbitMQ). Zbog OrderCreatedEvent 
            aktivira se DispatchDomainEventsInterceptor.

             */

        }
    }
}
