// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

// using stavio u GlobalUsing.cs

namespace Basket.API.Basket.GetBasket
{    /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
     
    //public record GetBasketRequest(string UserName);
    /* Ne treba Request object, jer argument mu je prostog tipa i mozemo ga kroz URL proslediti.
       Iako nema Request, Query u Handler klasi mora uvek da ima makar i bez argumenata. */
    public record GetBasketResponse(ShoppingCart Cart);
    /*Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u
    Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */

    public class GetBasketEndpoint : ICarterModule
    {
        // MOra ova metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {  /* Posto nema Request, vec argument preko URL prosledjujemo u Postman, moramo ga imati u rout
                * tj u "basket/{userName}" i bas istog imena mora biti "userName" kao u async(string userName,..).
                * MapGet jer samo citamo iz baze*/

                var result = await sender.Send(new GetBasketQuery(userName));
                // MediatR na osnovu GetBasketQuery zna da treba pozvati GetBasketQueryHandler
                var response = result.Adapt<GetBasketResponse>();

                return Results.Ok(response);

            }).WithName("GetBasketWithUsername")
              .Produces<GetBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("GetBasketWithUsername summary")
              .WithDescription("GetBasketWithUsername description");
        } /* U Postman, nema body, jer argument se prosledjuje kroz URL. */
    }
}
