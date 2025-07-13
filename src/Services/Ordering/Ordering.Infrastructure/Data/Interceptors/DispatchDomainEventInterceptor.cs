

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
        (Integration event) to RabbitMQ na kog je Ordering service Subscribed. 
        
        
         OrdrerUpdatedEvent flow (Domain event only): User aktivira UpdateOrderEndpoint koji ce aktivirati UpdateOrderCommandHandler koji ce pokrenuti Update metod iz Order koji ce dodati OrderUpdatedEvent u DomainEvents listu, a onda 
                                 dBContext.Orders.Update(order) ce aktivirati DispatchDomainEventInterceptor (zbog :SaveChangesInterceptor) koji ce da pokrene mediator.Publish(domainEvent) (IDomainEvent:INotification),a taj publishing
                                 ce da aktivira OrderUpdatedEvnetHandler automatski (zbog :INotifiicationEventHandler<OrderUpdatedEvent>). OrderUpdatedEventHandler nema telo jer sam resio da je tu kraj. 
        
         OrderCreatedEvent flow (integration leading to domain event): User aktivira CheckoutBasketEndpoint (koji sadrzi BasketCheckoutDTO) koji ce aktivirati CheckoutBasketCommandHandler koji ce (zbog IPublishEndpoint) koji ce da mapira BasketCheckoutDTO u BasketCheckoutEvent i  publishEndpoint.Publish(BasketCheckoutEvent), a taj
                                publishing ce da posalje BasketCheckoutEvent u RabbitMQ jer BasketCheckoutEvent: INtegrationEvent, a IntegrationEvent mora imati ista polja kao IDomainEvent jer se u RabbitMQ mapiraju. Obzirom da je Ordering.Application Consumer(Subscribed) na RabbitMQ i u MassTransit u BB je namesteno AddConsumer koje se odnosi 
                                samo na Ordering service, a BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent> i automatski se aktivira kad u RabbitMQ stigne BasketCheckoutEvent. BasketCheckoutEventHandler, mapira BasketCheckoutEvent u CreateOrderCommand kako bi, pomocu ISender, 
                                aktivirao CreateOrderCommandHandler automatski koji ce da pokrene Create metod iz Order koji ce dodati OrderCreatedEvent u DomainEvents listu , a onda dbContext.Orders.Add(order) koji ce aktivirati DispatchDomainEventInterceptor (zbog :SavingChangesInterceptor) koji ce da pokrene mediator.Publish(domainEvent) (IDomainEvent:INotification), 
                                a taj publishing ce da aktivira OrderCreatedEventHanlder automatski (zbog :INotificationEventHandler<OrderCreatedEvent>) i  onda OrderCreatedEventHandler salje OrderCreatedIntegrationEvent u RabbitMQ ako je OrderFullfilment feature ON
                                
         */

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
                await mediator.Publish(domainEvent); // Aktivira se OrdredCreatedEvenHandler automatski
        }
    }
}
