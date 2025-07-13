
using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Queries.GetOrders
{   // CQRS objasnjen u Catalog i Basket
    public record GetOrdersQuery(PaginationRequest paginationRequest) : IQuery<GetOrdersResult>;
    public record GetOrdersResult(PaginatedResult<OrderDTO> Orders); // Mora OrderDTO a ne Order zbog razdvanjaa Application i Domain layer i dobra je praksa 
}
