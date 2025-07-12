// using sam stavio u global using


using Catalog.API.DTOs;

namespace Catalog.API.Products.GetProductsByCategory
{   // Pogledaj CreateProductEndpoint tamo sve objasnjeno. OVde je Query pa nema validacija.
    public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IEnumerable<ProductResultDTO> products);
 
    public class GetProductsByCategoryQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {  
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().Where(p => p.Category.Contains(query.Category))
                                                         .Select(p => new ProductResultDTO { Name = p.Name, Category = p.Category, Price = p.Price, ImageFile = p.ImageFile, Description = p.Description }) // Od svakog Product pravim ProductResultDTO jer clientu ne valjda da vratim Product objekte jer je to tabela
                                                         .ToListAsync(cancellationToken);
            /* Zbog nemanja NoSQL baze, nemamo definisano ime tabele vec baza dodeli neko rangom, ali ga Query<Product> nadje 
            tabelu tipa Product, a onda nadje vrste kojima Category polje sadrzi rec iz query.Category stringa, pa pretvori to u Listu, 
            jer IEnumerable<Product> je argumet of Result. Mora Where, jer Category field u Product.cs nije PK. */
            return new GetProductsByCategoryResult(products);
        }
    }
}
