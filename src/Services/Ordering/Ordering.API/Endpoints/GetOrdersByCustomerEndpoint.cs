using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints
{   
    // ZNam iz Catalog i Basket CQRS 

    //public record GetOrdersByCustomerRequest(Guid CustomerId);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, GetOrdersByCustomerQuery mora postojati. */
    public record GetOrdersByCustomerResponse(IEnumerable<OrderDTO> Orders);

    public class GetOrdersByCustomerEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/orders/{customerId}
            app.MapGet("/orders/{customerId}", async (Guid customerId, ISender sender ) =>
            {
                var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));

                var response = result.Adapt<GetOrdersByCustomerResponse>();

                return Results.Ok(response); 

            }).WithName("GetOrdersByCustomer")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrdersByCustomer  summary ")
              .WithDescription("GetOrdersByCustomer desc");
        }
    }
}
