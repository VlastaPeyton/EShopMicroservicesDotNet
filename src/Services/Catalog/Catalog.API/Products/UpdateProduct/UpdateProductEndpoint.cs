// using sam stavio u global using

// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

namespace Catalog.API.Products.UpdateProduct
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    public record UpdateProductRequest(Guid Id, string Name, List<string> Category,
                                       string Description, string ImageFile, decimal Price);
    public record UpdateProductResponse(bool IsSuccess);

    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
            {   /* MapPut jer azuriramo. */

                var command = request.Adapt<UpdateProductCommand>();
                /* Mapiramo Request u Command da bi MediatR znao na osnovu UpdateProductCommand
                da poziva UpdateProductCommandHandler */
                var result = await sender.Send(command);
                // Mapiramo 
                var response = result.Adapt<UpdateProductResponse>();

                return Results.Ok(response);

            }).WithName("UpdateProduct")
              .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("UpdateProduct summary")
              .WithDescription("UpdateProduct description");
        }
        /* U Postman, pisemo ove argumente u body.*/
    }
}
