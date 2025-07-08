// using sam stavio u global using


namespace Catalog.API.Products.GetProductsByCategory
{   // Pogledaj CreateProductEndpoint tamo sve objasnjeno. OVde je Query pa nema validacija.
    public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IEnumerable<Product> Products);
 
    public class GetProductsByCategoryQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {  
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().Where(p => p.Category.Contains(query.Category)).ToListAsync();
            /* Zbog nemanja NoSQL baze, nemamo definisano ime tabele vec baza dodeli neko rangom, ali ga Query<Product> nadje 
            tabelu tipa Product, a onda nadje vrste kojima Category polje sadrzi rec iz query.Category stringa, pa pretvori to u Listu, 
            jer IEnumerable<Product> je argumet of Result. */
            return new GetProductsByCategoryResult(products);
        }
    }
}
