
namespace Ordering.Application.DTOs
{   // Objasnjeno u OrderDTO
    public record OrderItemDTO(
        Guid OrderId, // Jer OrderId Value polje je Guid
        Guid ProductId,
        int Quantity,
        decimal Price
        );
    /* OrderId polje u OrderItem.cs je OrderId.cs tipa koje ima Value polje tipa Guid i zato ovde samo Guid OrderId zbog JSON serialization/deserialization
       Isto vazi i za ProductId. */
}
