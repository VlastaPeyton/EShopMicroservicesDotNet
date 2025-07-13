
using System.Globalization;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // Customers tabela u bazi
    public class Customer : Entity<CustomerId> 
    {
        // Nasledio Id(CustomerId tipa jer radim Strongly-typed Id), CreatedAt, CreatedBy, ModifiedOn, ModifiedBy iz Entity
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        // Ova 2 polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodom van klase, dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
        // Zbog private set, moze new Customer = {...} 

        // Zbog Rich-domain dodajem Create static metodu umesto konstruktora kojom cu van klase moci da setujem Name, Email i Id polja.
        public static Customer Create(CustomerId id, string name, string email)
        {
            /* Validacija za Name i Email. Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB, a i da referencira ne bi imalo smisla jer nije Endpoint.*/

            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var customer = new Customer
            {
                Id = id, // Moro sam proslediti CustomerId u Create (nisam mogo CustomerId.Of(Guid.NewGuid()), zbog InitialData.cs kad pravim seeding podatke da bih mogo da iskoristim CustomerId iz customer tabele seedovan
                Name = name,
                Email = email
            };

            return customer;
        }
    }
}
