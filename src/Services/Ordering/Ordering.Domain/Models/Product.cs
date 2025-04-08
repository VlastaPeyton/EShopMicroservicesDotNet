

using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // U Catalog imam Product.cs, ali to nema veze sa ovim. 
    public class Product : Entity<ProductId>
    {
        // Nasledio Id(ProductId Guid), ModifedAt, ModifiedOn, CreatetBy, ModifiedBy iz Entity
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
        /* Polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodo
        dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
          Zbog Rich-domain dodajem Create static metodu umesto ktora gde cu i Id da popunim.   
        */
        public static Product Create (ProductId productId, string name, decimal price)
        {
            /*Validacija, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je
             Domain layer koji ga ne referncira plus ovo nije Endpoint. */
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var product = new Product
            {
                Id = productId,
                Name = name,
                Price = price
            };

            return product;
        }


    }
}
