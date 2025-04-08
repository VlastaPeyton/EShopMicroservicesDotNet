using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.API.Endpoints
{   
    public record CreateOrderRequest(OrderDTO Order);
    public record CreateOrderResponse(Guid Id);
    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi.
     
     U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */
    public class CreateOrderEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/orders", async (CreateOrderRequest request, ISender sender) =>
            {
                /* MapPost, jer upisujem novi order u bazu. Posto ima Request object, argument ne prosledjujem kroz URL u Postman,
                 */

                // Map incoming Request from client to Command object 
                var command = request.Adapt<CreateOrderCommand>();

                var result = await sender.Send(command);
                // MadiatR zna na osnovu CreateOrderCommand da trigueruje CreateOrderCommandHandler 

                // Map Result object to Response to send it to Client
                var response = result.Adapt<CreateOrderResponse>();

                return Results.Created($"/orders/{response.Id}", response); // 201 status code
                // Endpoint with route https://localhost:port/orders/{response.Id} bice defenisan jer mora da bude

            }).WithName("CreateOrder")
              .Produces<CreateOrderResponse>(StatusCodes.Status201Created) // Zbog Results.Created
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Order summary")
              .WithDescription("Create Order desc");
        }
    } /* U Postman body pisem imena argumenta iz OrderDTO klase zbog Request objecta.
         */
}
