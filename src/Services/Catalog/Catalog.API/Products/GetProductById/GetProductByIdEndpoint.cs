// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Endpoint radi kad Postman/Client ga gadja.
using Catalog.API.DTOs;

namespace Catalog.API.Products.GetProductById
{   // Pogledaj CreateProductEndpoint i CreateProductCommandHandler, da ne ponavljam. Jedina razlika sto je sad CQRS Query, pa nema Validacija. 

    //public record GetProductByIdRequest();
    // Nema Request, jer prosledjujemo samo Id argument to Endpoint, a posto je to Guid type, mozemo ga proslediti kroz URL ili Query

    public record GetProductByIdResponse(ProductResultDTO product);
    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/products/{id} GET 
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {  /* Nema Request object, a metoda zahteva argument, onda ga prosledjujem kroz URL ili Query. Kad prosledjujem kroz URL mora se isto "id" zvati arugment 
                i u "/products/{id}" i u async(Guid id,...). */

                var result = await sender.Send(new GetProductByIdQuery(id)); // result = GetProductByIdResponse
                // Na osnovu GetProductByIdQuery, MediatR zna da pozove Handle iz GetProductByIdQueryHandler
                var response = result.Adapt<GetProductByIdResponse>();

                return Results.Ok(response); // U FE, moram dohvatiti response.product ovaj ProductResultDTO koji nosi informaciju, jer Response object je samo wrapper

            }).WithName("GetProductById")
              .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Product By Id summary")
              .WithDescription("Get Product By Id description");  
        }
    }
}
