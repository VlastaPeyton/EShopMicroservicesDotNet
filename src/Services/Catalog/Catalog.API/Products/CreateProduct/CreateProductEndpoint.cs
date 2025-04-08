// Carter isntaliram u Catalog, a ne u BB, jer samo ovako Postman oce da raid

namespace Catalog.Api.Products.CreateProduct
{
    /* Client gadja Endpoint saljuci Request object. Endpoint prima taj Request, mapira ga u Command/Query, prosledjuje ga
     preko MediatR(Sender) u Handler.cs gde  odradjuju se radnje vezane za bazu i Handle vrati Result, a Endpoint ga mapira 
    u Response object koga salje kao odgovor clientu.*/
    public record CreateProductRequest(string Name, List<string> Category,
                                       string Description, string ImageFile,
                                       decimal Price);
    /* Sva polja iz Product, osim Id jer je Guid tipa, pa ce baza automatski da upise vrednost.
     Ali ostala polja mora klijent rucno da "upise". Znaci da u Postman body unosimo argumente iz CreateProductRequest u 
     JSON formatu */

    public record CreateProductResponse(Guid Id);
    // Kad uspesno modifikuje se baza, tj upise se novi product u nju, vracamo taj Id preko Response objekta.

    /* Request i Response object mora imati argumente istog imeta i tipa kao Query/Command i Result object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi. */

    public class CreateProductEndpoint : ICarterModule
    {
        // Mora ova metoda da se override zbog interface
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {   // MapPost, jer ovo je upisavanje novog producta u bazu

                // CQRS and MediatR lifecyclce slika objasnjava ova 3 koraka ispod

                var command = request.Adapt<CreateProductCommand>(); // type = CreateProductCommand 
                /* Client posalje Request u Endpoint, a Endpoint mapira(Mapster u BB uvezen) Request to Command object
                 jer na osnovu Command object, Sender (MediatR) zna da treba da aktivira Handle metodu iz Handler
                */
                var result = await sender.Send(command);
                // Sender aktivira Handle from CreateProductCommandHandler zbog typeof(command)
                var response = result.Adapt<CreateProductResponse>();
                // Handle metoda vraca Result (CreateProductResult) kog mapiramo u Response 

                return Results.Created($"/products/{response.Id}", response);
                /* return je oblika Minimal API, gde route = /products/{response.Id} tj ovo je povratna vrednost
                i moramo napraviti posebni Endpoint za ovu route. 

                  U Postman, Post method mora, url je https://localhost:5050/products, a body ce biti JSON oblika
                  { "Name": "ime", 
                    "Category": ['a','b','c'],
                    "Description": "opis",
                    "imageFile": "ImageFile Product docker",
                     "price": 200
                */
            }).WithName("CreateProduct")
              .Produces<CreateProductResponse>(StatusCodes.Status201Created)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Product summary")
              .WithDescription("Create Product description");
            // Ovo isto, samo drugacije sam radio u CollegeApp i zato znam sta je ovo.
        }
    }
}
