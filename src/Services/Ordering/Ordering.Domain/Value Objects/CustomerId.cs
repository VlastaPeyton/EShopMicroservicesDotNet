
using Ordering.Domain.DomainExceptions;

namespace Ordering.Domain.Value_Objects
{   
    // Strongly typed id 
    public record CustomerId
    {
        public Guid Value { get; }
        private CustomerId(Guid value) => Value = value;
        // Konstruktor zbog Of metode, samo krace napisan.
        /* 
           Polje nema set, sto znaci da moramo unutar recorda ga inicijalizovati
        kroz konstrutkor ili static metodu. 
        Zbog Rich-domain, Value Object ima static "Of metodu" umesto konstruktora */

        public static CustomerId Of(Guid value)
        {
            /* Validacija.  Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla
            jer niej Endpoint.*/
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
                throw new DomainException("CustomerId cannot be emptty");
            // Definisacu u DomainException posle

            return new CustomerId(value); 
        }
    }
}
