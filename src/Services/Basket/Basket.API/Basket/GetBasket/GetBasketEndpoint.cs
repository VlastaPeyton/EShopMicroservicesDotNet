// Carter isntaliram u Basket, a ne u BB, jer samo ovako Postman i Client oce dagadjaju Endpoint

using Basket.API.DTOs;

namespace Basket.API.Basket.GetBasket
{    // Pogledaj StoreBasketEndpoint i StoreBasketCommandHandler. Da ne ponavljam.
     
    //public record GetBasketRequest(string UserName);
    /* Ne treba Request object, jer argument mu je prostog tipa i mozemo ga kroz URL ili Query proslediti.
       Iako nema Request, Query u Handler klasi mora uvek da ima makar i bez argumenata. */
    public record GetBasketResponse(ReturnShoppingCartDTO cart); // Ne valjda Models klasu (tabelu) da vratim clientu, vec DTO. Kao u Catalog sto sam radio.

    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/basket/{userName} GET
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {  // Pogledaj CreateProductEndpoint i bice jasno zasto kroz URL prosledjujem argument

                var result = await sender.Send(new GetBasketQuery(userName));
                // MediatR na osnovu GetBasketQuery zna da treba pozvati GetBasketQueryHandler
                var response = result.Adapt<GetBasketResponse>();

                return Results.Ok(response);

            }).WithName("GetBasketWithUsername")
              .Produces<GetBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("GetBasketWithUsername summary")
              .WithDescription("GetBasketWithUsername description");
        } 
    }
}
