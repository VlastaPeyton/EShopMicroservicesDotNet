using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Querys.GetOrdersByName;

namespace Ordering.API.Endpoints
{   
    // Znam iz Catalog i Basket CQRS.

    //public record GetOrdersByNameRequest(string Name);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, GetOrdersByNameQuery mora postojati. */
    public record GetOrdersByNameResponse(IEnumerable<OrderDTO> Orders);
    
    public class GetOrdersByNameEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/orders/{orderName} GET
            app.MapGet("/orders/{orderName}", async (string orderName, ISender sender) =>
            {   
                var result = await sender.Send(new GetOrdersByNameQuery(orderName));
                
                var response = result.Adapt<GetOrdersByNameResponse>(); 

                return Results.Ok(response); 

            }).WithName("GetOrdersByName")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrdersByName  summary ")
              .WithDescription("GetOrdersByName desc");
        }
    }
}
