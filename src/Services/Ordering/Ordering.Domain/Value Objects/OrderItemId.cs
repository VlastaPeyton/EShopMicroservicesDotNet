

using Ordering.Domain.DomainExceptions;

namespace Ordering.Domain.Value_Objects
{   
    // Strongly typed id
    public record OrderItemId
    {
        public Guid Value { get; }
        private OrderItemId(Guid value) => Value = value;

        /*Polje nema set, sto znaci da moramo unutar recorda ga inicijalizovati
        kroz konstrutkor ili static metodu.
        Zbog Rich-domain, Value Object ima static "Of metodu" umesto konstruktora */

        public static OrderItemId Of(Guid value)
        {
            /* Validacija.  Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla
            jer niej Endpoint.*/
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
                throw new DomainException("OrderItemId cannot be empty");

            return new OrderItemId(value);
        }
    }
}
