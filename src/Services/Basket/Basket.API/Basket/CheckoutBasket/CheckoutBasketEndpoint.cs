
using Basket.API.Basket.StoreBasket;
using Basket.API.DTOs;
// Carter mora se instalira u Catalog.APi inace nece da radi Postman 
namespace Basket.API.Basket.CheckoutBasket
{   
    /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde odradjuju se radnje vezane za bazu i RabbitMQ i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu. Client navodi argumente of BasketCheckoutDTO. */
    public record CheckoutBasketRequest(BasketCheckoutDTO BasketCheckoutDto);
    // Koristim BasketCheckoutDTO, jer  razdvajam Application/API layer od Domain + ovime se postize sigurnost. BasketCheckoutDTO ima ista polja kao BasketCheckoutEvent.cs 
    public record CheckoutBasketResponse(bool IsSuccess);

    // Objasnjeno u StoreBasketEndpoint i StoreBasketCommandHandler.Da ne ponavljam.
    public class CheckoutBasketEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/basket/checkout POST
            app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<CheckoutBasketCommand>();
                var result = await sender.Send(command);
                // MediatR zna na osnovu CheckoutBasketCommand da pozove CheckoutBasketCommandHandler
                var response = result.Adapt<CheckoutBasketResponse>();

                return Results.Ok(response); 

            }).WithName("CheckoutBasket")
              .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Checkout Basket")
              .WithDescription("Checkout Basket");
        }
    }
}
