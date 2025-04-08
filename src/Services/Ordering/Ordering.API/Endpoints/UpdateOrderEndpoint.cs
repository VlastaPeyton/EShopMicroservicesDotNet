using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.API.Endpoints
{
    public record UpdateOrderRequest(OrderDTO Order);
    public record UpdateOrderResponse(bool IsSuccess);
    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi. 
      U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */

    public class UpdateOrderEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/orders/", async (UpdateOrderRequest request, ISender sender) =>
            {   /* MapPut, jer modifikujem postojeci order u bazu. Posto ima Request object, argument ne prosledjujem kroz URL u Postman,
                 */
                
                // Mapiram incoming Request from client to Command object
                var command = request.Adapt<UpdateOrderCommand>();

                var result = await sender.Send(command);
                // MediatR zna na osnovu UpdateOrderCommand da pozove UpdateOrderCommandHandler

                // Mapiram Result object to Response da vrati ga clientu
                var reponse = result.Adapt<UpdateOrderResponse>();

                return Results.Ok(reponse); // 200 status code 

            }).WithName("UpdateOrder")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Order summary ")
              .WithDescription("Update  Order desc");
        }
    } /* U Postman body pisem imena argumenta iz OrderDTO klase zbog Request objecta*/
}
