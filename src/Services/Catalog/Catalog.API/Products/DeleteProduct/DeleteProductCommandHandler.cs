//usings sam stavio u globalusing

using Catalog.API.Products.UpdateProduct;

namespace Catalog.API.Products.DeleteProduct
{   /* Prvo pogledaj CreateProductEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */

    public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi */

    /* U Command (nikad Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     * ovo je strongly typed validaton from MediatR.*/
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        // Konstruktor mora ovako zbog nasledjivanja
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is requiered");
        }
    }

    public class DeleteProductCommandHandler(IDocumentSession session)
        : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    { /* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocument session ima u sebi
        commit/rollback kao DbContext za SQL bazu sto ima.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            // Prvo se odradi valdicija jer sam je definisao u BuildingBlocks i dodao u MediatR pipeline u Program.cs 

            session.Delete<Product>(command.Id);
            /* Zbog NoSQL baze, nema ime tabele, ali Delete<Product> nadje tabelu tipa Product 
             */
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
