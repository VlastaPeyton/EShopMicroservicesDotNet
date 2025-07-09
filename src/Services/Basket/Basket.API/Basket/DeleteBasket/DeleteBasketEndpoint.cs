
namespace Basket.API.Basket.DeleteBasket
{    
    // Objasnjeno u StoreBasketEndpoint i StoreBasketCommandHandler. Da ne ponavljam. 

    //public record DeleteBasketRequest(string UserName);
    /* Ne treba Request object, jer ima 1 argument prostog tipa kog cu preko URL da prosledim u Postman. ALi bez obzira na ovo
     u Handler mora postojati Query object makar i bez argumenta.*/
    public record DeleteBasketResponse(bool IsSuccess);

    public class DeleteBasketEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/basket/{userName} DELETE
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
            {   // Objasnjeno u CreateProductCommandHandler zasto kroz URL ovde saljem argument 

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
