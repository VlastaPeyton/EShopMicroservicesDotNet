
// usings  sam stavio u GlobalUsing

namespace Catalog.Api.Products.CreateProduct
{
    /* Prvo pogledaj CreateProductEndpoint.cs i "CQRS and MediatR lifecyclce" sliku da bi razumeo ovo ispod.
     
       Command i Result object moraju imati argumente istog imena i tipa kao Request i Response objects, respketivno, kako bi mapiranje uspesno bilo.

       ICommand definisano u BuildingBlocks jer Catalog referencira to.
       
       U Command (nikad u Query) moram da validiram Command argumente, jer vrsim modifikaciju baze. Validacijom postizem razdvajanje API layer od Infrastructure
    u kome je EF Core u komunikaciji sa bazom. Catalog service nema zvanicno sve layere, vec samo "API" layer ali to nema veze, svakako validacija uvek kad upisujem nesto u bazu.
    Validacija (FluentValidation NuGet package) u BuildingBlocks je smestena. 
        
      Marten NuGet package omogucava LINQ za Postgre NoSQL + prednosti SQL Postre (Indexig) + pgAdming. Marten nije EF Core i zato Seeding DB ne moze preko Migration kao u Ordering sto ce biti,
    vec mora kao CatalogInitialData.cs da se uradi jer Seedujem Postgre NoSQL bazu. Koristim LightWeightDocumentSession for IDocumentSession koji nema Change Tracker automatski pa ako Add/Update u bazu,
    moram uvek pisati session.Store/Update beofre SaveChangesAsync kako bise promena u bazi sacuvalo.

      Namerno sam izbegao Repository pattern, pa je sva logika unutar Handle metode, jer Repository pattern i Vertical slice arhitektura ne idu zajedno.
     */
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);
 
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {   
        // Konstruktor mora ovako da se zove zbog nasledjivanja
        public CreateProductCommandValidator()
        {   // Kad neam FluentValidation, vec obicnu, umesto ovoga piseao bih u Product.cs [annotations] iznad svakog polja koje pomocu ModelState bih proveravao u Handle metodi.
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is requiered");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is requiered");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is requiered");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is > 0 mora");
            // Description polje recimo nema validaciju i zato ga ne pisem ovde. 
        }
    }

    public class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
    { /* ICommandHandler je u BuildingBlocks implementiran. 
       
        Koristim Primary Constructor da uvezem IDocumentSession (Marten NuGet package) DI umesto dosadasnjeg pravljenja polja private readonly IDocumentSession session koga u konstruktoru moram inicijalizovati. 
        
        IDocumentSession je pandan DbContext, samo za Marten Postgre NoSQL bazu. IDocumentSession registrujem u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. 
        IDocumentSession je ista stvar kao DbContext, samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocument session ima u sebi commit/rollback kao i DbContext.
        
        Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  

        CancellationToken se automatski aktivira ako client prekine process ili ako dodje do greske prilikom upisa u bazu.
     */

        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Prvo validaciju radim pre Handle, ali ona je definisana u BuildingBlocks i ubacena u MediatR pipeline u Program.cs 

            var product = new Product
            {
                // Id polje tipa Guid i baza ce sama da mu setuje vrednost, ne mi.
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
            };

            /* Tabela nece imati zeljeno ime (npr Products), jer ne koristim DbContext (koje cu u Ordering koristiti jer tamo imacu
             SQL bazu), vec IDocumentSession (koje je pandan DbContext, samo za Marten NoSQL) zbog Marten tj zbgo Postgres NoSQL,
             pa ce ime tabele biti random generisano, a na osnovu Store(product) tj typeof(product) IDocumentSession nadje tabelu Product
             tipa i tu upise novu vrstu(novi product). */

            // Save new product to DB using Marten. Store je pandan Add u DbContext. 
            session.Store(product); // Moram manuelno, jer koristim LightWeightDocumentSession koji nema Change Tracker i onda baza nece se azurirati nakon SaveChangesAsync ako ovo ne uradim.
            // baza ce automatski dodeliti novi Guid Id polju nakon unosa novog podatka u bazu.
            await session.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id); // Moze product.Id jer baza dodelima Guid Id polju automatski
        }
    }
}
