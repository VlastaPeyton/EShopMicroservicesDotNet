
namespace Ordering.Domain.Value_Objects
{   
    // Value object
    public record Address
    {
        public string FirstName { get; } = default!;
        public string LastName { get; } = default!;
        public string? EmailAddress { get; } = default!;
        public string AddressLine {  get; } = default!;
        public string Country { get; } = default!;
        public string State { get; } = default!;
        public string ZipCode { get; } = default!;

        /* Polja nemaju set, tj imaju internal set, pa moram unutar ove klase u konstruktoru ili u static metodu da setujem polja. 
          Zbog Rich-domain, koristim static "Of" metodu jer ovo je Value Object.*/

        // Polja nemaju explicitni "private set" => ne moze new Address {....} u Of metodi, vec mora private konstruktor koji cu koristiti u Of metodi
        private Address(string firstName, string lastName, string emailAddress,
                        string addressLine, string country, string state, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            Country = country;
            State = state;
            ZipCode = zipCode;
        }

        public static Address Of (string firstName, string lastName, string emailAddress,string addressLine, string country, string state, string zipCode)
        {
            /*Validacija mora ovde jer je ovo custom type, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je
             Domain layer koji ga ne referncira plus ovo nije Endpoint. */

            ArgumentException.ThrowIfNullOrEmpty(emailAddress);
            ArgumentException.ThrowIfNullOrEmpty(addressLine);
            ArgumentException.ThrowIfNullOrEmpty(country);
            ArgumentException.ThrowIfNullOrEmpty(state);
            ArgumentException.ThrowIfNullOrEmpty(zipCode);
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

            return new Address(firstName, lastName, emailAddress, addressLine, country, state, zipCode);

        }
    }
}
