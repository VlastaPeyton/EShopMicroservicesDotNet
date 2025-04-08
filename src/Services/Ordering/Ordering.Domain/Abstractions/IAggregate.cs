
namespace Ordering.Domain.Abstractions
{   
    /* Zbog DDD mora IAggregate koga ce da implementira Aggregate bas kao sto
    IEntity i IDomainEvent su nastali.*/
    public interface IAggregate : IEntity
    {  /* Implementira IEntity, jer Aggregate Root ima Id, a IEntity ima Id polje + treba mu
        i audit kolone koje IEntity ima. 
        * Posto je IAggregate interface, ne mora da implementira nista iz IEntity, ali
        Aggregate klasa koja ce da implementira IAggregate morace sve iz IEntity i IAggregate. */

        // To handle Domain Events

        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        /* Nisam stavio "set;" jer ne zelim da DomainEvents omogucim da pokazuje na drugu listu. 
         Kad dohvatim ovo polje izvan klase, ne mogu ga modifikovati elemente liste  zbog IReadOnlyList.
           
           Mala digresija zbog IReadOnlyList: 
        public readonly List<IDomainEvents> DomainEvents {get;} znaci da ne mogu menjati DomainEvents da 
        referencira na drugu listu je nema "set;", ali elemente te liste mogu menjati ! 
            
           IDomainEvent cu definisati kasnije, jer se kao i do sad, zbog DDD, radi sve preko interface, pa tek 
        prave se klase (OrderCreatedevent i OrderUpdatedEvent) koje implements ovaj interface. */
        IDomainEvent[] ClearDomainEvents();
        /* Kroz DependencyInjection.cs cu da registrujem da zna da se IDomainEvent odnosi 
         * na DomainEvent jer ce DomainEvent da implementira IDomainEvent. */
    }

    // Zbog generic T u IEntity<T>, moramo imati i IAggregate<T> 
    public interface IAggregate<T>: IEntity<T>, IAggregate
    {
        /* Posto je ovo interface, ne mora da implementira nista iz IAggregate i IEntity<T>
         vec klasa (Aggregate<T>) koja ovo implementira morace da implementira sve iz IAggregate i IEntity<T>.
        */
    }
}
