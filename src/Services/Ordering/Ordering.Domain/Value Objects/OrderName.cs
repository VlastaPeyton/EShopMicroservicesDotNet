
using Ordering.Domain.DomainExceptions;

namespace Ordering.Domain.Value_Objects
{   
    // Strongly typed class
    public record OrderName
    {
        public string Value { get; }
        private const int DefaultLength = 5;
        private OrderName(string value) => Value = value;

        /*Polje nema set, sto znaci da moramo unutar recorda ga inicijalizovati
        kroz konstrutkor ili static metodu.
        Zbog Rich-domain, Value Object ima static "Of metodu" umesto konstruktora */

        public static OrderName Of(string value)
        {
            /* Validacija.  Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla
            jer niej Endpoint.*/
            ArgumentException.ThrowIfNullOrEmpty(value);
            //ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength); skidam jer cesto zaboravim pa da ne bude greska u postman

            return new OrderName(value);
        }
    }
}
