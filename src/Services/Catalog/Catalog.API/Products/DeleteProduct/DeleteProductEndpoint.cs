
// usings sam stavio u global using 
// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

namespace Catalog.API.Products.DeleteProduct
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    
    //public record DeleteProductRequest(Guid id);
    //necemo Request, jer id je tipa Guid, pa mozemo i kroz URL da ga prosedimo
    public record DeleteProductResponse(bool IsSuccess);

    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */

    public class DeleteProductEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}", async (Guid id, ISender sender) =>
            {  /* Nema Request, pa smo id kroz URL prosledili, ali mora onda isto ime "id"
                * biti u route /products/{id} i u async(Gudi id,...) */

                var result = await sender.Send(new DeleteProductCommand(id));
                // Zbog DeleteProductCommand MediatR zna da treba da pozove DeleteProductCommandHandler
                var response = result.Adapt<DeleteProductResponse>();

                return Results.Ok(response);

            }).WithName("DeleteProduct")
              .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("Delete Product")
              .WithDescription("Delete Product");

        }
    }
}
