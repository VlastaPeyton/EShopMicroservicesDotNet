using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints
{    
    // Objasnjenje u Catalog i Basket imam za  CQRS.

    //public record DeleteOrderRequest(Guid Id);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, DeleteOrderCommand mora postojati. */
    public record DeleteOrderResponse(bool IsSuccess);

    public class DeleteOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/orders{id}
            app.MapDelete("/orders/{id}", async(Guid id, ISender sender) =>
            {
               
                var result = await sender.Send(new DeleteOrderCommand(id));

                var response = result.Adapt<DeleteOrderResponse>();

                return Results.Ok(response); // 200 status code

            }).WithName("DeleteOrder")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete Order summary ")
              .WithDescription("Delete  Order desc");
        }
    }
}
