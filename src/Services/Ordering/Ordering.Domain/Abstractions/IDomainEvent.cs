
using MediatR;

namespace Ordering.Domain.Abstractions
{   /* Zbog DDD pravimo IDomainEvent koga ce da implementira DomainEvent.
     Bas kao IEntity sto ga Entity implementirao.*/
    public interface IDomainEvent : INotification
    {   // INotification je MediatR dopusta prenos of DomainEvent kroz mediator handler

        Guid EventId => Guid.NewGuid(); // Samo prilikom get ovu vrednost sracuna

        public DateTime OccuredOn => DateTime.Now;  // Samo prilikom get ovu vrednost sracuna

        public string EventType => GetType().AssemblyQualifiedName; // Samo prilikom get ovu vrednost sracuna
        /* Returns Type object koji predstavlja runtime type of the current instance (class where this code is definedd)
         */
    }
}
