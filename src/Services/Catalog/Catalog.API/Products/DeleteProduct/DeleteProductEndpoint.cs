// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Endpoint radi kad Postman/Client ga gadja.

namespace Catalog.API.Products.DeleteProduct
{   // Pogledaj CreateProductEndpoint i CreateProductCommandHandler, da se ne ponavljam ovde. 
    
    //public record DeleteProductRequest(Guid id);
    // Ne treba Request object, jer id je tipa Guid, pa mogu i kroz URL Query Parameters da ga posaljem 
    public record DeleteProductResponse(bool IsSuccess);
    public class DeleteProductEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   
            // https://localhost:port/products/{id} DELETE
            app.MapDelete("/products/{id}", async (Guid id, ISender sender) =>
            {  // Nema Request, pa smo id kroz URL prosledili, ali mora onda isto ime "id" biti u "/products/{id}" i u async(Gudi id,...) 

                // Nema mapiranje from Request to Command, jer nema Request object. 
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
