// using sam stavio u global using


using Marten.Pagination;

namespace Catalog.API.Products.GetProducts
{   /* Prvo pogledaj GetProductsEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10): IQuery<GetProductsResult>;
    // Query mora da postoji bez argumenata ako Response u Endpoint ne postoji i ako ne postoji argument in URL
    public record GetProductsResult(IEnumerable<Product> Products);
    /* Zelim da vrati listu (koja implementira IENumerable) svih producta iz baze i zato ovakav argument
     Ovo je dobra praksa da pisemo uvek interface, dok u Handle metodi listu vracamo. 
    
     Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */
    public class GetProductsQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductsQuery, GetProductsResult>
    {   /* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocumentSession ima u sebi
        commit/rollback kao i DbContext sto ima za SQL bazu*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {  
            // CancelationToken sluzi da se prekine proces ako dodje do greske prilikom citanja iz bazu
           
            var products = await session.Query<Product>().ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);
            /* Query<Product> ide u bazu i nadje tabelu tipa Product, jer tabela nema definisano ime, jer NoSQL baza. 
            */
            return new GetProductsResult(products);
        }
    }
}
