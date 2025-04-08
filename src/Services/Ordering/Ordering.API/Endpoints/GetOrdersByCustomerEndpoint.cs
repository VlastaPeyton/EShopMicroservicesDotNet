using Carter;
using Mapster;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints
{
    //public record GetOrdersByCustomerRequest(Guid CustomerId);
    /* Argument je prostog tipa i mozemo ga kroz URL u Postman proslediti pa ne treba Request object, ali iako nema 
     Request object, GetOrdersByCustomerQuery mora postojati. */
    public record GetOrdersByCustomerResponse(IEnumerable<OrderDTO> Orders);

    /*Request i Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u
     respektivno, kako bi mapiranje moglo da se izvrsi. 
      U Clean architecture, pomocu DTO razdvajamo Applicatio/API layer od Domain cime postizemo i sigurnost. */
    public class GetOrdersByCustomerEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{customerId}", async (Guid customerId, ISender sender ) =>
            {
                /* Nema Request object, argument saljem kroz URL u Postman, ali zato mora  isto ime argumenta "customerId" biti
                 u route (/orders/{customerId}) i u async(Guid customerId). */

                // Nema mapiranje iz Request u Query object jer Request nema

                var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));
                // MediatR zna zbog GetOrdersByCustomerQuery da treba zvati GetOrdersByCustomerQueryHandler

                // Map from Result object to Response koji ce da se posalje clientu
                var response = result.Adapt<GetOrdersByCustomerResponse>();

                return Results.Ok(response); // 200 statuss code

            }).WithName("GetOrdersByCustomer")
              .Produces<UpdateOrderResponse>(StatusCodes.Status200OK) // Zbog Results.Ok
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound) // AKo ne nadje ni jedan Order
              .WithSummary("GetOrdersByCustomer  summary ")
              .WithDescription("GetOrdersByCustomer desc");
        }
    }
}
