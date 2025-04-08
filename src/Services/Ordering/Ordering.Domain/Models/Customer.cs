
using System.Globalization;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{
    public class Customer : Entity<CustomerId> // Customers tabela u bazi jer nasledio Entity
    {
        // Nasledio Id(CustomerId tipa), CreatedAt, CreatedBy, ModifiedOn, ModifiedBy iz Entity

        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        
        /* Polja su private set, moram ih inicijalizovati unutar klase  ili konstruktorom
        ili static metodom kao kod Order.
           Zbog Rich-domain, koristim static metodu "Create" */

        public static Customer Create (CustomerId customerId, string name, string email)
        {
            /* Validacija za Name i Email. Ne mogu koristiti BuildingBlocks za FluentValidaiton in MediatR
             jer ovo nije Endpoint vec Domain layer koji ne referencira BB a i da referencira ne bi imalo smisla
            jer niej Endpoint.*/

            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var customer = new Customer
            {
                Id = customerId, // Nasledio Id iz Entity.cs tipa CustomerId
                Name = name,
                Email = email
            };

            return customer;
        }
    }
}
