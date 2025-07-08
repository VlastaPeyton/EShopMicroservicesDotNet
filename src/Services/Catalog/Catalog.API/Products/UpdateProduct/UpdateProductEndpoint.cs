// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

namespace Catalog.API.Products.UpdateProduct
{   // Pogledaj CreateProductEndpoint i CreateProductCommandHandler da se ne ponavljam. 
    public record UpdateProductRequest(Guid Id, string Name, List<string> Category,
                                       string Description, string ImageFile, decimal Price);
    public record UpdateProductResponse(bool IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   
            // https://localhost:port/products PUT
            app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
            {   // UpdateProductRequest from Request Body saljem. 

                var command = request.Adapt<UpdateProductCommand>();
                // Mapiramo Request u Command da bi MediatR znao na osnovu UpdateProductCommand da pozove UpdateProductCommandHandler
                var result = await sender.Send(command);
         
                var response = result.Adapt<UpdateProductResponse>();

                return Results.Ok(response);

            }).WithName("UpdateProduct")
              .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("UpdateProduct summary")
              .WithDescription("UpdateProduct description");
        }
    }
}
