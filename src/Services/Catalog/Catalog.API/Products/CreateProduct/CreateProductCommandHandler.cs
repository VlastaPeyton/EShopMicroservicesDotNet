
// usings  sam stavio u GlobalUsing

namespace Catalog.Api.Products.CreateProduct
{
    /* Prvo pogledaj CreateProductEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */

    // Ovo je Command object sa slike iznad 
    public record CreateProductCommand(string Name, List<string> Category,
                                       string Description, string ImageFile,
                                       decimal Price) : ICommand<CreateProductResult>;
    /* Sva polja iz Product, osim Id, jer Id je tipa Guid, pa baza ce to autoamtski da upise. 
     Ali ostala polja, kao sto znam, sam upisem (preko clienta). 
       IRequest<CreateProductResult> jer uvek u <> mora da stoji Result object kao na slici iznad.
     ICommand je  u BuildingBlocks jer Catalog referencira ga.*/

    // Ovo je Result object sa slike iznad 
    public record CreateProductResult(Guid Id);
    /* Kad se CreateProductCommand izvrsi, u bazi dodeli mi se automatski Id koga vracamo klijentu preko CreateProductResult 
    tj CreateProductResponse u CreateProductEndpoint.cs jer Endpoint direktno komunicira sa client. */

    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi */

    /* U Command (nikad Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     * ovo je strongly typed validaton from MediatR.*/
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {   
        // Ova konstruktor mora ovako da se zove zbog nasledjivanja
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is requiered");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is requiered");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is requiered");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is >0 mora");
            // Description ne podrazumevam npr kao obavezno polje i onda nema RuleFor za njega

        }
    }
    public class CreateProductCommandHandler(IDocumentSession session)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    { /* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocument session ima u sebi
        commit/rollback kao DbContext za SQL bazu sto ima.
        
         U Program.cs cu dodati AddValidatorsFromAssembly zbog IValidator - ovo sam izbrisao iz ove klase pa cu valjda i iz Program.cs*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // CancelationToken sluzi da se prekine proces ako dodje do greske prilikom upisa u bazu

            // Prvo validaciju radim, ali ona je definisana u BuildingBlocks i ubacena u MediatR pipeline u Program.cs 

            var product = new Product
            {
                // Id polje tipa Guid i baza ce sama da mu setuje vrednost, ne mi.
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
            };

            // Save new product to DB using Marten 
            /* Tabela nece imati zeljeno ime (npr Products), jer ne koristim DbContext (koje cu u Ordering koristiti jer tamo imacu
             SQL bazu), vec IDocumentSession (koje je pandan DbContext, samo za Marten NoSQL) zbog Marten tj zbgo Postgres NoSQL,
             pa ce ime tabele biti random generisano, a na osnovu Store(product) tj typeof(product) IDocumentSession nadje tabelu Product
             tipa i tu upise novu vrstu(novi product). */
            session.Store(product); // baza ce automatski dodeliti Guid Id polju.
            await session.SaveChangesAsync(cancellationToken);
            return new CreateProductResult(product.Id); // Jer baza dodelima Guid Id polju automatski
        }
    }
}
