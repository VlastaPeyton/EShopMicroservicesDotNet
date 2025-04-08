
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
        
        /* Polja nemaju set, tj imaju internal set, pa moram unutar ove klase u konstruktoru
        ili u static metodu da setujem.
          Zbog Rich-domain, koristim static "Of" metodu jer ovo je Value Object.*/
        
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
        // Construktor private zbog Of metode

        public static Address Of (string firstName, string lastName, string emailAddress,
                        string addressLine, string country, string state, string zipCode)
        {
            /*Validacija, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je
             Domain layer koji ga ne referncira plus ovo nije Endpoint. */

            ArgumentException.ThrowIfNullOrEmpty(emailAddress);
            ArgumentException.ThrowIfNullOrEmpty(addressLine);

            return new Address(firstName, lastName, emailAddress, addressLine, country, state, zipCode);

        }
    }
}
