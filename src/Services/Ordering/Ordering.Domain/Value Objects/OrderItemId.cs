

using Ordering.Domain.DomainExceptions;

namespace Ordering.Domain.Value_Objects
{   
    // Strongly typed id
    public record OrderItemId
    {
        public Guid Value { get; }
        private OrderItemId(Guid value) => Value = value; // Konstruktor na moderan nacin jer imam 1 polje samo

        /* Value polje nema set (tj implicitni private set ima), sto znaci da moramo unutar recorda ga inicijalizovati kroz konstrutkor ili static metodu.
        Zbog Rich-domain, Value Object ima static "Of metodu" umesto konstruktora da mogu van klase da setujem ovo polje.
          
            Zbog internal set, ne moze new CustomerId{Value=value} vec bi trebao private konstruktor recimo*/

        public static OrderItemId Of(Guid value)
        {
            /* Validacija se radi u klasi jer ovo je custom type. Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla jer niej Endpoint.*/
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
                throw new DomainException("OrderItemId cannot be empty");

            return new OrderItemId(value);
        }
    }
}
