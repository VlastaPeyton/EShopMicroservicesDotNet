// Carter isntaliram u Basket, a ne u BB, jer samo ovako Postman/ Client moze da gadja Endpoints

using Basket.API.DTOs;

namespace Basket.API.Basket.StoreBasket
{   // Objasnjeno u CreateProductEndpoint i CreateProductCommandHandler u Product service. Da se ne ponavljam.
    public record StoreBasketRequest(StoreShoppingCartDTO Cart);
    public record StoreBasketResponse(string UserName);
    
    public class StoreBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/basket POST
            app.MapPost("/basket", async (StoreBasketRequest request, ISender sender) =>
            { 
                var command = request.Adapt<StoreBasketCommand>(); 
                var result = await sender.Send(command);
                // Sender zna na osnovu typeof(command)=StoreBasketCommand da pozove StoreBasketCommandHandler
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/basket/{response.UserName}", response);
                /* Frontendu ce biti poslato response object u Response Body, StatusCode=201 u Response Status Line, a https://localhost:port/basket/{UserName} u Response Header.
                  Moram napraviti Endpoint za https://localhost:port/basket/{UserName} obzirom da ga ovde saljem korisniku, a to je GetBasketEndpoint.

                  Mora Results.Created, jer neam IActionResult kao u Controller pa da moze samo Created. 
               */

            }).WithName("StoreBasket")
              .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("StoreBasket summary")
              .WithDescription("Store Basket description");
        }
    }
}
