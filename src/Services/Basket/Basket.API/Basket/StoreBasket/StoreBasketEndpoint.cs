// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid
// using je u GlobalUsing

namespace Basket.API.Basket.StoreBasket
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    public record StoreBasketRequest(ShoppingCart Cart);
    public record StoreBasketResponse(string UserName);
    /*Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u
    Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class StoreBasketEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket", async (StoreBasketRequest request, ISender sender) =>
            {
                /* MapPost jer upisujemo novi ShoppingCart u bazu. 
                 * Request object postoji, stoga u Postman moramo pisati JSON u body sa poljima iz ShoppingCart klase*/

                var command = request.Adapt<StoreBasketCommand>(); // Adapt je iz Mapster
                var result = await sender.Send(command);
                // Sender zna na osnovu typeof(command)=StoreBasketCommand da pozove StoreBasketCommandHandler
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/basket/{response.UserName}", response);
                /* Mora da se napravi Endpoint za route= "/basket/{response.UserName}", a to je vec napravljeno
                 u GetBasketEndpoint. */

            }).WithName("StoreBasket")
              .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("StoreBasket summary")
              .WithDescription("Store Basket description");
        }
    }
}
