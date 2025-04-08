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
    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi. 
      U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */
    public class GetOrdersEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {   /* Nema Request object, argument saljem kroz URL u Postman, ali zato mora [AsParameters] jer slozen je tip
                  */
                // Nema mapiranje iz Request to Query jer Request nema 
                
                var result = await sender.Send(new GetOrdersQuery(request));
                // MediatR zna na osnovu GetOrdersQuery da poziva GetOrdersQueryHandler

                // Mapiram iz Result object to Result object koji ce poslat biti clientu
                var response = result.Adapt<GetOrdersResponse>();

                return Results.Ok(response); // 200 status code

            }).WithName("GetOrders")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrders  summary ")
              .WithDescription("GetOrders desc");
        }
        
    }
}
