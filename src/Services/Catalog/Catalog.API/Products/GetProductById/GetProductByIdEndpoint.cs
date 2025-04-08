//usings sam stavio u globalusing
// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

namespace Catalog.API.Products.GetProductById
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    
    //public record GetProductByIdRequest();
    /*Nema Request, jer prosledjujemo samo Id argument to Endpoint, a posto je to Guid type, 
    mozemo ga proslediti kroz URL */
    
    public record GetProductByIdResponse(Product Product);
    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class GetProductByIdEndpoint : ICarterModule
    {
        // MOra ova metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {  /*MapGet jer citam iz baze. Posto nema Request, a metoda zahteva argument, onda ga 
                prosledjujem kroz URL. Vodi racuna, kad prosledjujem kroz URL, mora se isto "id" zvati arugment 
                i u route (/products/{id}) i u async(Guid id,...). */

                var result = await sender.Send(new GetProductByIdQuery(id));
                // Na osnovu GetProductByIdQuery, MediatR zna da pozove Handle iz GetProductByIdQueryHandler
                var response = result.Adapt<GetProductByIdResponse>();

                return Results.Ok(response);

            }).WithName("GetProductById")
              .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Product By Id summary")
              .WithDescription("Get Product By Id description");  
        }
        /* Zbog nemanja Request, ali slanja argumenta kroz URL, u Postman neam body, ali ima argument u URL */
    }
}
