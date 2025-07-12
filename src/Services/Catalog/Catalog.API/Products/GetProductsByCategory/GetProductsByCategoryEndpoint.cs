// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman i Client oce da gadjaju Endpoint

using Catalog.API.DTOs;

namespace Catalog.API.Products.GetProductsByCategory
{  // Pogledaj CreateProductEndpoint i CreateProductCommandHandler, da ne ponavljam. Jedina razlika sto je sad CQRS Query, pa nema Validacija. 

    //public record GetProductsByCategoryRequest();
    // Nema Request, jer string Category je prost tip i mozemo proslediti preko URL ili Query parameters.
    public record GetProductsByCateogryResponse(IEnumerable<ProductResultDTO> products);
    public class GetProductsByCategoryEndpoint : ICarterModule
    {   
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/products/category/{category} GET
            app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
            {  /* Posto nema Request, a metoda zahteva argument, onda ga prosledjujem kroz URL recimo. Vodi racuna, kad prosledjujem kroz URL, mora se isto "category" zvati arugment 
                i u "/products/{category}" i u async(string category,...). */
                var result = await sender.Send(new GetProductsByCategoryQuery(category));
                /* Na osnovu GetProductsByCategoryQuery, MediatR zna da treba da zove GetProductsByCategoryQueryHandler */
                var response = result.Adapt<GetProductsByCateogryResponse>();

                return Results.Ok(response);

            }).WithName("GetProductsByCategory")
              .Produces<GetProductsByCateogryResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Products By Category summary")
              .WithDescription("Get Products By Category descritpion");
        } 
    }
}
