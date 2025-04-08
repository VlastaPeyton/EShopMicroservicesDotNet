// using sam stavio u global using
// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid


namespace Catalog.API.Products.GetProductsByCategory
{   /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    
    //public record GetProductsByCategoryRequest();
    // Nema Request, jer string Category je prost tip i mozemo proslediti preko URL. 

    public record GetProductsByCateogryResponse(IEnumerable<Product> Products);
    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */
    public class GetProductsByCategoryEndpoint : ICarterModule
    {   
        // Mora metoda zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
            {     /* MapGet jer Query. Posto nema Request, a metoda zahteva argument, onda ga 
                prosledjujem kroz URL. Vodi racuna, kad prosledjujem kroz URL, mora se isto "category" zvati arugment 
                i u route (/products/{category}) i u async(string category,...). */
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
        /* Zbog nemanja Request, vec samo URL argument, u Postman nema body. */
    }
}
