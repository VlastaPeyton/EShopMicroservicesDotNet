// using sam stavio u global using


namespace Catalog.API.Products.GetProductsByCategory
{   /* Prvo pogledaj GetProductByIdEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IEnumerable<Product> Products);
    /*Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Handler klasi, respektivno, kako bi mapiranje moglo da se izvrsi

     Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */
    public class GetProductsByCategoryQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {   /* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocumentSession ima u sebi
        commit/rollback bas kao i DbContext za SQL bazu.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
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
