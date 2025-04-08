using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints
{   
    //public record DeleteOrderRequest(Guid Id);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, DeleteOrderCommand mora postojati. */
    public record DeleteOrderResponse(bool IsSuccess);
    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi. 
      U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */

    public class DeleteOrderEndpoint : ICarterModule
    {
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/orders/{id}", async(Guid id, ISender sender) =>
            {
                /* Nema Request object, argument saljem kroz URL u Postman, ali zato mora  isto ime argumenta "id" biti
                 u route (/orders/{id}) i u async(Guid id). */

                // Nema mapiranje Request to Command jer Request object nema 
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
