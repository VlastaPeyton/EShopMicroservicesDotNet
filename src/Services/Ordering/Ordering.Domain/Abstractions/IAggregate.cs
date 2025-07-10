
namespace Ordering.Domain.Abstractions
{   
    // Zbog DDD mora IAggregate koga ce da implementira Aggregate bas kao sto su IEntity i IDomainEvent za Entity i DomainEvent.
    public interface IAggregate : IEntity
    {  /* Implementira IEntity, jer Aggregate Root treba Id, a IEntity ima Id polje. Takodje, treba mu i audit kolone koje IEntity ima. 
        Posto je IAggregate interface, ne mora da implementira nista iz IEntity, ali  klasa koja ce da implementira IAggregate morace sve iz IEntity i IAggregate. */

        public IReadOnlyList<IDomainEvent> DomainEvents { get; }
        /* Nisam stavio "set;" jer ne zelim da Aggregate moze da modifikuje DomainEvents nikako osim pomocu  ClearDomainEvenets i AddDomainEvents(bice def u Aggregate).
         Kad dohvatim ovo polje izvan klase, ne mogu mu modifikovati elemente liste zbog IReadOnlyList. Poenta je u Aggregate klasi definisati DomainEvents kao expression bodied getter 
        videcu ovo kako radi u Aggregate.
           
           Mala digresija zbog IReadOnlyList: 
        public readonly List<IDomainEvents> DomainEvents {get;} znaci da ne mogu menjati DomainEvents da 
        referencira na drugu listu je nema "set;", ali elemente te liste mogu menjati ! 
            
        IDomainEvent, jer ko za IEntity i Entity, dobra praksa preko interface, plus imam OrderCreatedEvent i OrderUpdatedEvent, a DomainEvents ne moze da bude oba tipa, pa zato interface im pravim.
         */
        public IDomainEvent[] ClearDomainEvents(); 
    }

    // Zbog generic T u IEntity<T>, moramo imati i IAggregate<T> 
    public interface IAggregate<T>: IEntity<T>, IAggregate
    {
        // Posto je ovo interface, ne mora da implementira nista iz IAggregate i IEntity<T>, vec klasa (Aggregate<T>) koja ovo implementira morace da implementira sve iz IAggregate i IEntity<T>.
    }
}
