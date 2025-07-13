

using BuildingBlocks.CQRS;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Querys.GetOrdersByName
{    
    
    public record GetOrdersByNameQuery(string Name) : IQuery<GetOrdersByNameResult>;
    public record GetOrdersByNameResult(IEnumerable<OrderDTO> Orders); // OrderDTO a ne Order da razdvojim Domain od Application layer
  

}
