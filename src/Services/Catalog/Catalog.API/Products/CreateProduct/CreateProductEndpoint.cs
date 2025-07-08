// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Endpoint radi kad Postman/Client ga gadja.

namespace Catalog.Api.Products.CreateProduct
{
    /* Client gadja Endpoint saljuci Http Request. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje to
     preko MediatR(Sender) u odgovarajuci Handler gde odradjuju se radnje vezane za bazu i Handler vrati Result, a Endpoint to mapira 
    u Response object koga salje kao odgovor clientu.
    
        Client u Request Body / URL / Query  mora proslediti ista imena ovih polja iz CreateProductRequest. Znaci da u Postman body unosimo argumente iz CreateProductRequest u 
    JSON formatu + imena polja moraju biti ista kao u CreateProductRequest. Ako Request nema puno argumenata i svi su prostog tipa, ne mora postojati Request object, vec mozemo 
    ih poslati preko Query Parameters in URL from Frontend.
    
        CQRS ne sadrzi DTO objects (kao sto bi bilo u slucaju bez CQRS), vec Reqeust i Response objects koji su record type. 
        
        Request, Response object mora imati argumente istog imena i tipa kao Query/Command i Result object u Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi.
    
        ICarterModule (Carter NuGet package) omogucava kreiranje Minimal API Endpoint bez Controller. 

        ISender (MediatR NuGet package) omogucava koristecenje Mediator patterna zbog mapiranja Request u Command/Query i Result u Response. 
        
        Adapt (Mapster NuGet package) omogucava mapiranje Request to Command/Query i Result to Response objekata.
        
        Sto se tice app.MapMethod stvari stoje ovako. Nemamo [FromBody], [FromRoute], [FromQuery] kao u Controller. Vec imamo:
            // Route param "id" binds from URL 
            app.MapGet("/items/{id}", (int id) => { ... });

            // Query param "page" binds from query, e.g. /items?page=2
            app.MapGet("/items", ([As{arameters] CreateProductRequest request) => { ... });

            // Complex type binds from JSON body by default
            app.MapPost("/items", async (CreateProductRequest request) => { ... });
    */
    public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
   
    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        // Mora ova metoda da se override zbog interface.
        public void AddRoutes(IEndpointRouteBuilder app)
        {   
            // https://localhost:port/products POST 
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {   // MapPost, jer ovo je upisavanje novog producta u bazu. Isto kao [HttpPost] u Controller. 
                // CreateProductRequest se salje kroz Request Body jer MapPost je ovakav. 

                // CQRS and MediatR lifecyclce slika objasnjava ova 3 koraka ispod

                var command = request.Adapt<CreateProductCommand>(); // typeof(command) = CreateProductCommand 
                /* Client posalje Request u Endpoint, a Endpoint mapira (Mapster u BB imported) Request to Command object
                 jer na osnovu Command object, Sender zna da treba da aktivira Handle metodu iz zeljenog Handler. */

                var result = await sender.Send(command); // typeof(result) = CreateProductResult
                // Sender aktivira Handle from CreateProductCommandHandler zbog typeof(command) = CreateProductCommand 

                var response = result.Adapt<CreateProductResponse>(); // typeof(response) = CreateProductResponse 
                // Handle metoda vraca Result (CreateProductResult) kog mapiram u Response.

                return Results.Created($"/products/{response.Id}", response);
                /* Frontendu ce biti poslato response u Response Body, StatusCode=201 u Response Status Line, a https://localhost:port/product/{id} u Response Header.
                   Moram napraviti Endpoint za https://localhost:port/product/{id} obzirom da ga ovde saljem korisniku. 
                    
                   Mora Results.Created, jer neam IActionResult kao u Controller pa da moze samo Created. 
                */
            
            // Ovo je dokumentacija sluzi samo kad testiram u Postman/Swagger. Ne salje se ovo clientu. 
            }).WithName("CreateProduct")
              .Produces<CreateProductResponse>(StatusCodes.Status201Created)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Product summary")
              .WithDescription("Create Product description");
        }
    }
}
