
using Ordering.Domain.Abstractions;
using Ordering.Domain.Models;

namespace Ordering.Domain.Events
{
    // Event je record (ili mala klasa) koja sluzi da obavesti da se nesto desilo.
    public record OrderCreatedEvent(Order Order) : IDomainEvent;
    // IDomainEvent ima EventId, OccuredOn and EventType polja, ali su ona expression-bodied getteri tamo pa ovde ne moram da ih override ako necu
    
}
