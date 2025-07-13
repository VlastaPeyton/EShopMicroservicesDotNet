
using System.Xml.Linq;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // Child Entity from Order Aggregate. Ovo je OrderItems tabela u bazi.
    public class OrderItem : Entity<OrderItemId> 
    {
        // Nalsedili smo Id(OrderItemId tipa jer radim Strongly-typed Id), CreatedAt, ModifiedOn, ModifiedBy i CreatedBy iz Entity
        public OrderId OrderId { get; private set; } = default!; // FK. Povezuje Id of Order.cs  jer 1 OrderItem pripada 1 Order-u, a List<OrderItem> je navigational attribute u Order pa zato ovde mora OrderId.
        public ProductId ProductId { get; private set; } = default!; // FK
        // Povezuje Id of Product.cs jer 1 OrderItem je 1 Product ali 1 Product moze biti selektovan vise puta pa onda 1 Product bude vise OrderItems-a 
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;

        // Ova polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodom van klase, dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
        // Zbog private set, moze new OrderItem = {...}  ali nisam to koristio 

        // Zbog Rich-domain dodajem Create static metodu umesto konstruktora kojom cu van klase moci da setujem polja
        public static OrderItem Create(OrderId id, ProductId productId, int quantity, decimal price)
        {
            // Validacija, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je Domain layer koji ne referncira BB plus ovo nije Endpoint.
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            return new OrderItem { 
                        Id = OrderItemId.Of(Guid.NewGuid()),
                        OrderId = id,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = price
            };
        }
    }
}
