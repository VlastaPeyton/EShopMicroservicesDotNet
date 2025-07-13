
using BuildingBlocks.CQRS;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{   
    public record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<GetOrdersByCustomerResult>;
    public record GetOrdersByCustomerResult(IEnumerable<OrderDTO> Orders); // Mora OrderDTO a ne Order jer dobra praksa razdvojiti Domain i Applicaiton layer
}
