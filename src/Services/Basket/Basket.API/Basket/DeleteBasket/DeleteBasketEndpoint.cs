// Carter mora se instalira u Catalog.APi inace nece da radi Postman 


namespace Basket.API.Basket.DeleteBasket
{    /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/

    //public record DeleteBasketRequest(string UserName);
    /* Ne treba Request object, jer ima 1 argument prostog tipa kog cu preko URL da prosledim u Postman. ALi bez obzira na ovo
     u Handler mora postojati Query object makar i bez argumenta.*/
    public record DeleteBasketResponse(bool IsSuccess);
    /*Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u
    Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class DeleteBasketEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
            {
                /* MapDelete, jer brisemo iz baze. Posto nema Request object, argument prosledjujem kroz URL u postman,
                 i zato u rout="/bakset/{userName}" mora pisati isto ime objekta kao u async(string userName,...) */

                var result = await sender.Send(new DeleteBasketCommand(userName));
                // MediatR zna na osnovu DeleteBasketCommand da treba pozvati DeleteBasketCommandHandler
                var response = result.Adapt<DeleteBasketResponse>();

                return Results.Ok(response);

            }).WithName("DeleteBasket")
              .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("Delete Basket summary")
              .WithDescription("Delete Basket desc");
        }
    }
}
