
//usings sam stavio u globalusing

namespace Catalog.API.Products.GetProductById
{   // Pogledaj CreateProductEndpoint, da ne ponavljam. Jedina razlika sto je sad CQRS Query, pa nema Validacija. 
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product Product);

    public class GetProductByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {  
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(query.Id, cancellationToken); // Moze LoadAsync by Id, jer Id field u Product.cs je PK
            /* U bazi nadje tabelu tipa Product i za prosledjeni Id nadje zeljenu vrstu. Zbog NoSQL baze, nema ime tabele,
            vec LoadAsync<Product> nadje zeljenu vrstu u tabeli tipa Product. */
            if (product is null)
                throw new ProductNotFoundException(query.Id); // Def u Catalog/Exceptions folder

            return new GetProductByIdResult(product);
        }
    }
}
