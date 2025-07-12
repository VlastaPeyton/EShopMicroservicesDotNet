
// usings sam stavio u globalusing   
using Catalog.API.DTOs;
using Catalog.API.Extensions;

namespace Catalog.API.Products.GetProductById
{   // Pogledaj CreateProductEndpoint, da ne ponavljam. Jedina razlika sto je sad CQRS Query, pa nema Validacija. 
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>; 
    public record GetProductByIdResult(ProductResultDTO product);  // Koristim DTO, jer ne smem da vratim Product klasu jer je ona "tabela"(collection) u bazi.

    public class GetProductByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {  
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {   
            var product = await session.LoadAsync<Product>(query.Id, cancellationToken); // Moze LoadAsync by Id, jer Id field u Product.cs je PK
            // typeof(product) = Product

            /* U bazi nadje tabelu tipa Product i za prosledjeni Id nadje zeljenu vrstu. Zbog NoSQL baze, nema ime tabele,
            vec LoadAsync<Product> nadje zeljenu vrstu u tabeli tipa Product. */
            if (product is null)
                throw new ProductNotFoundException(query.Id); // Def u Catalog/Exceptions folder

            return new GetProductByIdResult(product.ToGetProductByIdResultDTO()); // Jer ne valja da vratim Product klasu koja predstavlja bazu vec DTO klasu sa zeljenim poljima iz Product
        }
    }
}
