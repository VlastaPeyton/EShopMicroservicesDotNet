
using MediatR;

namespace Ordering.Domain.Abstractions
{   /* Zbog DDD pravimo IDomainEvent koga ce da implementira svaki Domain Event (OrderCreatedEvent i OrderUpdatedEvent).
     Ne samo zbog DDD, vec u IAggregate DomainEvents lista sadrzi oba tipa eventa,a to moze samo ako lista bude tipa interface koga nasledjuju oba tipa. Isto objasnjenje i za AddDomainEvents u Aggregate.*/
    public interface IDomainEvent : INotification
    {   // INotification je iz MediatR da dopusta prenos i primanje of DomainEvent kroz Mediator . Videti DispatchDomainEventsInterceptor.
        Guid EventId => Guid.NewGuid(); // Samo prilikom get ovu vrednost sracuna

        public DateTime OccuredOn => DateTime.Now;  

        public string EventType => GetType().AssemblyQualifiedName; 
        // Returns Type object koji predstavlja runtime type of the current instance (class where this code is definedd). Obirom da OrderCreatedEvent i OrderUpdatedEvent ce ovo implementirati, bice Ordering.Domain.Events.OrderCreated/UpdatedEvent
    }
    // Expression-bodied polja u interface ne moraju biti definisana u klasi koja ga implementira !!!!!!
    // Obicna polja moraju biti definisana u klasi koja ga implementira !!!! 
}
