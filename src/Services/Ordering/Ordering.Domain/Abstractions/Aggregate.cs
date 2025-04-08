
namespace Ordering.Domain.Abstractions
{   
    // Ovo je i Aggregate i Aggregate Root
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
    { // Mora da nasledi Entity<T>, jer Aggregate ima Id polje i audit kolone
        // Sva polja iz Entity (Id, CreatedAt, ModifiedOn, CreatedBy i ModifiedBy su nasledjena 

        /* Moram ovako definisati _domainEvents, jer ta linija mi treba da napravim
        expression-bodied property gett koji se racuna samo kad dohvatim ga. 
        
          private readonly List<IDomainEvent> _domainEvents - _domainEvents ne moze da referencira drugu listu
        ali elemetni liste na koju pokazuje mogu da se promene. Ocu private, jer mi je bitno. 
          
          IReadOnlyList<IDomainEvent> DomainEvents - (implementirano iz IAggregate) ne moze se menjati 
        sadrzaj DomainEvenets liste, ali moze se menjati pokazivac na sta ce DomainEvents pokazivati. U IAggregate
        DomainEvents ima samo {get;} jer ne zelim da DomainEvents moze da pokazuje na nesto drugo ako mu prosledim tako
        i zato ovde ce morati bas ove 2 linije ispod. 
         */

        // Moram implementirati sve metode i polja iz IAggregate 
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        /* Da sam stavio get umesto "=> _domainEvents.AsReadonly()" jer omogućava svojstvo bez set i osigurava 
        da se kolekcija ne može menjati spolja. 
        
           Kroz DependencyInjection.cs cu da registrujem da zna da se DomainEvent odnosi na IDomainEvent, jer ce 
        DomainEvenet da implementira IDomainEvent. */

        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequedEvents = _domainEvents.ToArray();
            _domainEvents.Clear(); // List ima Clear kao built-in
            // Moze Clear, jer readonly List<IDomainEvent> sto omogucava modifikaciju liste ali ne i pokazivaca na nju
            return dequedEvents; // Prazan niz tj nema vise DomainEvents 
        }

        public void AddDomainEvent (IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent); // reaonly List<IDomainEvnet> dopusta modifikaciju liste, ali ne i pokazivaca na nju
        }

    }
}
