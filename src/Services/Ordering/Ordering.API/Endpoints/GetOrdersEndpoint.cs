using BuildingBlocks.Pagination;
using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints
{
    //public record GetOrdersRequest(PaginationRequest paginationRequest);
    /* Argument nije prostog tipa, ali ga opet mozemo ga kroz URL u Postman proslediti pa ne treba Request object, 
     ali iako nema Request object, GetOrdersRequestQuery mora postojati. */
    public record GetOrdersResponse(PaginatedResult<OrderDTO> Orders);
    
    public class GetOrdersEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   //https://localhost:port/orders GET
            app.MapGet("/orders/", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {  
                
                var result = await sender.Send(new GetOrdersQuery(request));

                var response = result.Adapt<GetOrdersResponse>();

                return Results.Ok(response);

            }).WithName("GetOrders")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrders  summary ")
              .WithDescription("GetOrders desc");
        }
        
    }
}
