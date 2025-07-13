using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.API.Endpoints
{
    public record UpdateOrderRequest(OrderDTO Order);
    public record UpdateOrderResponse(bool IsSuccess);

    public class UpdateOrderEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/orders/", async (UpdateOrderRequest request, ISender sender) =>
            {   
                var command = request.Adapt<UpdateOrderCommand>();

                var result = await sender.Send(command);

                var reponse = result.Adapt<UpdateOrderResponse>();

                return Results.Ok(reponse); // 200 status code 

            }).WithName("UpdateOrder")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Order summary ")
              .WithDescription("Update  Order desc");
        }
    } 
}
