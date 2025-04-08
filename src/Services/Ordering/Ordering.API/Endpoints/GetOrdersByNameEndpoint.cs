using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Querys.GetOrdersByName;

namespace Ordering.API.Endpoints
{
    //public record GetOrdersByNameRequest(string Name);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, GetOrdersByNameQuery mora postojati. */
    public record GetOrdersByNameResponse(IEnumerable<OrderDTO> Orders);
    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi. 
      U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */
    public class GetOrdersByNameEndpoint : ICarterModule
    {   
        // Mora metoda zbog itnerface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{orderName}", async (string orderName, ISender sender) =>
            {
                /* Nema Request object, argument saljem kroz URL u Postman, ali zato mora  isto ime argumenta "orderName" biti
                 u route (/orders/{orderName}) i u async(string orderName). */

                // Nema mapiranje Request to Command jer Request object nema 
                
                var result = await sender.Send(new GetOrdersByNameQuery(orderName));
                
                // Mapiram iz Result object to Response object koji ce se poslati clientu
                var response = result.Adapt<GetOrdersByNameResponse>(); 

                return Results.Ok(response); // 200 status code

            }).WithName("GetOrdersByName")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrdersByName  summary ")
              .WithDescription("GetOrdersByName desc");
        }
    }
}
