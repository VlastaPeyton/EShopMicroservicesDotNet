
using Basket.API.DTOs;

namespace Basket.API.Basket.CheckoutBasket
{   // Carter mora se instalira u Catalog.APi inace nece da radi Postman 
    /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/

    public record CheckoutBasketRequest(BasketCheckoutDTO BasketCheckoutDto);
    /* Koristim BasketCheckoutDTO, jer Ordering (Integration Event Subscriber) ima Clean architecture gde razdvajam Application/API layer od Domain + 
     ovime se postize sigurnost. BasketCheckoutDTO ima ista polja kao BasketCheckoutEvent.cs  */
    public record CheckoutBasketResponse(bool IsSuccess);

    /*Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u
   Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class CheckoutBasketEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
            {
                // Map from Request object to Command object 
                var command = request.Adapt<CheckoutBasketCommand>();

                var result = await sender.Send(command);
                // MediatR zna na osnovu CheckoutBasketCommand da pozove CheckoutBasketCommandHandler

                // Map from Result object to Reponse koji ce poslat biti clientu
                var response = result.Adapt<CheckoutBasketResponse>();

                return Results.Ok(response); // 200 status code

            }).WithName("CheckoutBasket")
              .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Checkout Basket")
              .WithDescription("Checkout Basket");
        }
        /* U Postman body pisem argumente od BasketCheckoutDTO */
    }
}
