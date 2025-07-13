using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.API.Endpoints
{   // Iz Catalog i Basket znam CQRS i kako sve ovo radi. OrderDTO jer razdvajam Applicaiton od Domain layera
    public record CreateOrderRequest(OrderDTO Order);
    public record CreateOrderResponse(Guid Id);
    
    // Pogledaj OrderCreatedCommandHandler za FeatureManagement objasnjenje ima veze sa ovim Endpoint.
    public class CreateOrderEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/orders POST
            app.MapPost("/orders", async (CreateOrderRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateOrderCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateOrderResponse>();

                return Results.Created($"/orders/{response.Id}", response); 

            }).WithName("CreateOrder")
              .Produces<CreateOrderResponse>(StatusCodes.Status201Created) // Zbog Results.Created
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Order summary")
              .WithDescription("Create Order desc");
        }
    }
}
