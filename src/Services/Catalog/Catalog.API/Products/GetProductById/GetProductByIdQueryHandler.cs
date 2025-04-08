
//usings sam stavio u globalusing

namespace Catalog.API.Products.GetProductById
{   /* Prvo pogledaj GetProductByIdEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product Product);
     /*
     Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */

    public class GetProductByIdQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {   /* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocumentSession ima u sebi
        commit/rollback kao DbContext za SQL bazu sto ima.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(query.Id, cancellationToken);
            /* U bazi nadje tabelu tipa Product i za prosledjeni Id nadje zeljenu vrstu. Zbog NoSQL baze, nema ime tabele,
            vec LoadAsync<Product> nadje zeljenu vrstu u tabeli tipa Product. */
            if (product == null)
                throw new ProductNotFoundException(query.Id); // Def u Catalog/Exceptions folder

            return new GetProductByIdResult(product);
        }
    }
}
