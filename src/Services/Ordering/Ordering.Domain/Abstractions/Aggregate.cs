
namespace Ordering.Domain.Abstractions
{   
    // Ovo je i Aggregate i Aggregate Root. Zelim osigurati da ne moze postojati objekat ove klase i zato abstract (kao za Entity).
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
    { 
        /* Iako IAggregate<TId> :IEntity<TId>, opet bih morao polja iz IEntity da definisem ovde. Zato Aggregate<TId>:Entity<TId>, da ta polaj ne moram ovde definisati.
        Aggregate treba Id i audit kolone opciono. I zato nasledio sam Entity<TId> da ne morah te kolone ovde definisati.
        
            Razliku  readonly List<IDomainEvent> i ReadOnlyList<IDomainEvent> pogledaj u IAggregate.cs => _domainEvenets moze da modifukuje elemnte liste, ali ne moze da referencira drugu listu.
            
            _domainEvents mora biti private, jer Order (koji nasledi Aggregate) ne sme da dohvati direktno ovo polje. Zato pravim public IReadOnlyList DomainEvents polje koje Order moze da dohvati
        plus ono je expression bodied getter sto znaci da samo prilikom dohvatanja DomainEvenets se izracuna _domainEvnets. 

            DomainEvents (moram implementirati iz IAggregate) mora biti expression-bodied getter jer u IAggregate nema set jer ne zelim da Aggregate moze bilo kako, osim pomocu nasledjenog ClearDomainEvents i svog AddDomainEvents, modifikuje listu!!
        Dok _domainEvents.AsReadOnly mora kako Order(koji nasledjuje Aggregate) ne bi mogo da modifikuje _domainEvents elemente liste.

          IReadOnlyList<IDomainEvent> DomainEvents - (implementirano iz IAggregate) ne moze se menjati 
        sadrzaj DomainEvenets liste, ali moze se menjati pokazivac na sta ce DomainEvents pokazivati. U IAggregate
        DomainEvents ima samo {get;} jer ne zelim da DomainEvents moze da pokazuje na nesto drugo ako mu prosledim tako
        i zato ovde ce morati bas ove 2 linije ispod. 
         */

        // Moram implementirati sva polja iz  IAggregate (DomainEvents i ClearDomainEvents) i mogu naravno dodati svoje sta ocu
        private readonly List<IDomainEvent> _domainEvents = new();  // Ovu listu menjam po potrebi. Dok DomainEvents sluzi da dohvatim listu bez mogucnosti menjaja van ove klase.
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly(); // Navigational attribute ali ga ne gledamo tako jer je ovo abstract klasa koja ce biti nasledjena  + ovo je event, a neamo tabelu za events pa da treba PK-FK veza u OnModelCreating da se definise

        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequedEvents = _domainEvents.ToArray();
            _domainEvents.Clear(); // List ima Clear kao built-in da joj izbrisem sve elemente. 
            // Moze Clear, jer readonly List<IDomainEvent> sto omogucava modifikaciju liste ali ne i pokazivaca na nju
            return dequedEvents; // Prazan niz tj nema nijedan DomainEvent
        }

        public void AddDomainEvent (IDomainEvent domainEvent)
        {   // IDomainEvenet, jer postoje 2 tipa of Domain Event (OrderCreatedEvent i OrderUpdatedEvent) i zato interface nam treba jer ne moze argument biti 2 tipa istovremeno osim ako nije interface.
            _domainEvents.Add(domainEvent); // reaonly List<IDomainEvnet> dopusta modifikaciju liste, ali ne i pokazivaca na nju
        }

    }
}
