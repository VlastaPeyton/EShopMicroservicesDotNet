// using sam stavio u global using
// Carter instaliram u Catalog, jer ako instliram u BB, nece Postman hteti. 
namespace Catalog.API.Products.GetProducts
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/

    //public record GetProductsRequest();
    /*jer za ovaj Query ne treba Request object, posto dohvatamo sve producte iz baze
    bice slucajeva kad ce za Query imati Request ili samo argumente u URL saljemo. 
     Ali sada koristicu Request jer dodajem Pagination kroz URL parametre. */
    public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);

    public record GetProductsResponse(IEnumerable<Product> Products);
    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class GetProductsEndpoint : ICarterModule
    {   
        // MOra ova metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
            {  /* MapGet, jer citamo iz baze 
               Posto sad ima Request object, parametri su mu prostog tipa i mozemo kroz URL ih proslediti
                onda dodao [AsParameters] (nisam morao) da me podseti da request ne ide u Postman body, vec
                PageNumber i PageSize u URL idu. */

                var query = request.Adapt<GetProductsQuery>(); 
                var result = await sender.Send(query);
                /* MediatR(Sender) na osnovu GetProductsQuery zna da mora pozove Handle metodu iz 
                GetProductsQueryHandler*/
                var response = result.Adapt<GetProductsResponse>();

                return Results.Ok(response);

            }).WithName("GetProducts")
              .Produces<GetProductsResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Products summary")
              .WithDescription("Get Products description");
            /* Posto nema Request, a niti MapGet nema argument za URL, onda u Postman pisem samo 
              URL bez body i bez URL argumenta. */ 
        }
    }
}
