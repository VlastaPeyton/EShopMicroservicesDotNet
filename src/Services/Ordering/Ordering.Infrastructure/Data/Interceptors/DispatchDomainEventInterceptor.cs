

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.Domain.Abstractions;

namespace Ordering.Infrastructure.Data.Interceptors
{   
    // Dispatch = Publish Domain Events in Ordering microservice
    public class DispatchDomainEventInterceptor(IMediator mediator) : SaveChangesInterceptor
    {   /* u DepdencyInjection.cs (Program.cs) mora da se registruje  da DispatchDomainEventInterceptor 
         se odnosi na ISaveChangesInterceptor>. 
        
           IMediator je pandan IPublishEndpoint u CheckoutBasketCommandHandler u Basket koji publish BasketCheckoutEvent
        (Integration event) to RabbitMQ na kog je Ordering Subscribed. */

        // Kucam public override i ponudi nam metode i izaberemo ove 2 

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            /* Extract Domain Events from Aggregate (Orrder.cs) and dispatch them, jer u Aggregate imamo OrderCreatedEvent i OrderUpdatedEvent
              
               GetAwaiter().GetResult() pravi sync from async, jer DispatchDomainEvents je async, dok SavingChanges mora biti sync, 
            pa zato morali da prebacimo from async to sync. 
               
               eventData.Context = ApplicationDbContext*/
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, 
                                                                                    CancellationToken cancellationToken = default)
        {
            /* SavingChangesAsync je asyn i zato nema GetAwaiter().GetResult() kao iznad, ali mora await jer je DispatchDomainEvents async */
            await DispatchDomainEvents(eventData.Context); // eventData.Context = ApplicationDbContext
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public async Task DispatchDomainEvents(DbContext? context)
        {   // ApplicationDbContext nasledila DbContext a znam da uvek roditelja stavljam zbog DDD
            if (context is null)
                return;

            /* Koristim IDomainEvent, a ne DomainEvent, u skladu sa DDD. 
               Trazim DbContext entitete koji implementiraju IAggregate, a to je Order.cs tj Orders tabela.
               DomainEvents je polje u IAggregate.cs  */
              
            // Retrieve  enttites of type IAggregate (Order.cs) that have DomainEvents associated with them */
            var aggregates = context.ChangeTracker.Entries<IAggregate>() // Sve tabele (klase) koje su nasledile IAggregate.
                                                 .Where(a => a.Entity.DomainEvents.Any()) // DomainEvents je polje u Order.cs
                                                 .Select(a => a.Entity); //  svaki entry(vrsta) u OrderItems listi valjda ?

            // Retrieve DomainEvents from Aggregates (Order.cs)
            var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

            /* Avoid duplicate Dispatch i zato moram koristim ClearDomainEvents metodu koja je potpisana u IAggregate
             Kada smo izvukli u varijablu sve DomainEvents onda praznimo DomainEvents listu */

            /* Postoje 2 vrste Events: 
                1) Domain Event  - Domain Event je Published(Dispatched) i Consumed unutar istog service (Ordering u mom slucaju) i zato 
                                   koristimo MediatR jer je unutar istog servisa. 
                                   Domain Event se Publish kad se desi modifikacija u bazi (moze i za reading from db, ali je retko).
                                   Za Domain Event kazem Dispatch(Publish)/Consume, a za Integration Event samo Publish/Consume.
                                   Imamo OrderCreatedEvent i OrderUpdatedEvent kao Domain Events def u Domain layer.

                2) Integration Event - bice kasnije o ovome, ali cu napisati ovde da bih skapirao ovaj kod.
                                       Integration Event je Published from Service1 (Basket u mom slucaju) i Consumed in Service2 
                                       (Ordering u mom slucaju). Publish-Consume(Subscribe) se radi preko Message Broker (RabbitMQ 
                                       + MassTransit library). 
                
              mediator.Publish(domainEvent) ce da aktivira OrderCreatedEventHandler ili OrdredUpdatedEventHandler koji je nasledio 
            INotificationHandler<OrderCreatedEvent> ili INotificationHandler<OrderUpdatedEvent>, repsketivno, jer OrderCreatedEvent i 
            OrderUpdatedEvent su nasledil INotification i zato event moze biti publishovan, a zbog INoficationHandler<..>  MediatR 
            znace za koji domain event handle metodu da aktivira.  

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
            foreach (var domainEvent in domainEvents) 
                await mediator.Publish(domainEvent); // Aktivira se OrdredCreatedEvenHandler 
        }
    }
}
