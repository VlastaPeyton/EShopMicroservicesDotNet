

using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // U Catalog imam Product.cs, ali to nema veze sa ovim. Ovo je Products tabela u bazi.
    public class Product : Entity<ProductId>
    {
        // Nasledio Id(ProductId tipa jer radim Strongly-typed Id), ModifedAt, ModifiedOn, CreatetBy, ModifiedBy iz Entity.cs
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
        // Ova 2 polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodom van klase, dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
        // Zbog private set, moze new Product = {...} 

        // Zbog Rich-domain dodajem Create static metodu umesto konstruktora kojom cu van klase moci da setujem Name,Price i Id polja.
        public static Product Create (ProductId id, string name, decimal price)
        {
            // Validacija, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je Domain layer koji ne referncira BB plus ovo nije Endpoint.
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var product = new Product
            {
                Id = id, // Mozda i ne moram proslediti id, jer moze Id = ProductId.Of(Guid.NewGuid())
                Name = name,
                Price = price
            };

            return product;
        }
    }
}
