
using Ordering.Domain.Abstractions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // Child Entiyty from Order Aggregate
    public class OrderItem : Entity<OrderItemId> // OrderItems tabela u bazi jer nasledio Entity
    {
        // Nalsedili smo Id(OrderItemId tipa), CreatedAt, ModifiedOn, ModifiedBy i CreatedBy iz Entity
        
        // Nasledjen Id predstavlja OrderItemId 
        public OrderId OrderId { get; private set; } = default!; // Povezuje Id of Order.cs  jer 1 OrderItem pripada 1 Orderu. Orders tabela u bazi.
        public ProductId ProductId { get; private set; } = default!; 
        /* Povezuje Id of Product.cs jer 1 OrderItem je 1 Product ali 1 Product moze biti selektovan vise puta pa onda 1 Product bude vise OrderItems
        Products tabela u bazi.
        Umesto Guid, pravim OrderId i ProductId tipove, jer to je dobra praksa da svako id polje ima svoj tip  - strongly typed id  */
        public int Quantity { get; private set; } = default!;

        public decimal Price { get; private set; } = default!;

        // Konstruktor (ili static metoda kao u Order) , jer su private set polja
        public OrderItem(OrderId orderId, ProductId productId, int quantity, decimal price)
        {   
            /* Nasledjeno Id(OrderItemId tipa) polje je public set u Entity.cs, pa ga mogu setovati gde ocu, ali ja cu ovde. 
             * OrderItemId je strongly typed id (kao za OrderId i ProductId) koji ima static metodu "Of" */
            Id = OrderItemId.Of(Guid.NewGuid()); 
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
    }
}
