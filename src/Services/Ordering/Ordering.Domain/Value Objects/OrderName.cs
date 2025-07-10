
using Ordering.Domain.DomainExceptions;

namespace Ordering.Domain.Value_Objects
{   
    // Strongly typed class
    public record OrderName
    {
        public string Value { get; }
        private const int DefaultLength = 5;
        private OrderName(string value) => Value = value; // Konstruktor na moderan nacin jer imam 1 polje samo

        /* Value polje ima internal set, sto znaci da moramo unutar recorda ga inicijalizovati kroz konstrutkor ili static metodu.
        Zbog Rich-domain, Value Object ima static "Of metodu" umesto konstruktora da mogu van klase da setujem ovo polje.
          Zbog internal set, ne moze new CustomerId{Value=value} vec bi trebao private konstruktor recimo*/

        public static OrderName Of(string value)
        {
            /* Validacija se radi ovde jer je custom type.  Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla jer niej Endpoint.*/
            ArgumentException.ThrowIfNullOrEmpty(value);
            //ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength); skidam ovo jer cesto zaboravim pa da ne bude greska u postman ili u client

            return new OrderName(value);
        }
    }
}
