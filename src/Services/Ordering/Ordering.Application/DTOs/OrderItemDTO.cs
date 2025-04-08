
namespace Ordering.Application.DTOs
{   /* U entity klasi Order.cs, OrderItem element liste  je tipa OrderItem.cs koje ima vise polja i 
     zato sva polja iz OrderItem mapiram u OrderItemDTO.*/
    public record OrderItemDTO(
        Guid OrderId,
        Guid ProductId,
        int Quantity,
        decimal Price
        );
    /* OrderId polje u OrderItem.cs je OrderId.cs tipa koje ima Value polje tipa Guid i zato ovde samo Guid OrderId
       Isto vazi i za ProductId. */
}
