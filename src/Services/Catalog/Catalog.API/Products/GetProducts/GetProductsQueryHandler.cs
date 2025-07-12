// using sam stavio u global using


using Catalog.API.DTOs;
using Marten.Pagination;

namespace Catalog.API.Products.GetProducts
{   
    public record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10): IQuery<GetProductsResult>;
    // Query mora da postoji bez argumenata ako Response u Endpoint ne postoji i ako ne postoji argument koji se salje from client
    public record GetProductsResult(IEnumerable<ProductResultDTO> products); // Mora DTO jer razdvajam Domain layer (Product) of Application layer tj klijentu ne treba Product klasa vec njen DTO
    // Zelim da vrati listu (koja implementira IENumerable) svih producta iz baze i zato ovakav argument. Ovo je dobra praksa da pisemo uvek interface, dok u Handle metodi listu vracamo. 
    
    public class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {   
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {  
            // CancelationToken sluzi da se prekine proces ako dodje do greske prilikom citanja iz bazu
           
            var products = await session.Query<Product>().Select(p => new ProductResultDTO { Name = p.Name, Category = p.Category, Description = p.Description, ImageFile = p.ImageFile, Price = p.Price}).ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);
            // Query<Product> nadje tabelu tipa Product, jer tabela nema definisano ime, jer NoSQL baza. I za svaki Product napravi ProductResultDTO jer ne valja clientu da vratim Product jer je to Domain tj tabela

            return new GetProductsResult(products);
        }
    }
}
