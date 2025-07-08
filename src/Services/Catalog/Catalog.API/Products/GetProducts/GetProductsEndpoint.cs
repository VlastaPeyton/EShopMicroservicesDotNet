// Carter instaliram u Catalog, jer ako instliram u BB, nece Postman hteti. 
namespace Catalog.API.Products.GetProducts
{   // Pogledaj CreateProductEndpoint i CreateProductCommandHandler, da ne ponavljam. Jedina razlika sto je sad CQRS Query, pa nema Validacija. 
    public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);
    public record GetProductsResponse(IEnumerable<Product> Products);
 
    public class GetProductsEndpoint : ICarterModule
    {   
        // MOra ova metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/products GET
            app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
            {  // [AsParameters] allows you to group multiple query string parameters into a single object and use that object in Minimal API.

                var query = request.Adapt<GetProductsQuery>(); 
                var result = await sender.Send(query);
                // MediatR(Sender) na osnovu GetProductsQuery zna da mora pozove Handle metodu iz GetProductsQueryHandler
                var response = result.Adapt<GetProductsResponse>();

                return Results.Ok(response);

            }).WithName("GetProducts")
              .Produces<GetProductsResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Products summary")
              .WithDescription("Get Products description");
        }
    }
}
