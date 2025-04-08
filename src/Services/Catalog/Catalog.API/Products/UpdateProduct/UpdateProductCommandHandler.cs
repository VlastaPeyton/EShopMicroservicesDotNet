// using sam stavio u global using

using Catalog.Api.Products.CreateProduct;

namespace Catalog.API.Products.UpdateProduct
{   /* Prvo pogledaj CreateProductEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    
    public record UpdateProductCommand(Guid Id, string Name, List<string> Category, 
                                       string Description, string ImageFile, decimal Price)
               : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);

    /* U Command (nikad Query)  moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     * ovo je strongly typed validaton from MediatR.*/
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        // Konstruktor mora ovako zbog nasledjivanja
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is requiered");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category is requiered")
                                .Length(2, 150).WithMessage("Name must be [2,150]");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price is >0 mora");
            // Category, Description, Image file nisam hteo da validiram eto tako
        }
    }

    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi */
    public class UpdateProductCommandHandler(IDocumentSession session)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {/* ICommandHandler je u BuildingBlocks jer Catalog referencira ga.
        Pomocu Primary Constructor importujem session. Mogao sam i napraviti polje 
        private readonly IDocumentSession session, pa da napravim konstruktor klase gde cu
        da ovom polju dodelim tu vrednost, ali nema potrebe jer ne getterujem ovo van klase. 
        
         IDocumentSession cu registrovati u Program.cs da kod prepoznaje ovo kao DocumentSession, a ne 
        kao IDocumentSession. U Program.cs takodje napisacu a koristim LightWeightSession. IDocumentSession je ista stvar kao
        DbContext u CollegeApp samo sto je DbContext za SQL, a IDocumentSession za Postgre NoSQL bazu. IDocumentSession ima u sebi
        commit/rollback kao DbContext sto ima za SQL bazu.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila CreateProductResult 
         u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {            
            // Prvo ce se odraditi validacija, jer sam definisao je u BuildingBlocks + dodao u MediatR pipeline. 

            var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
            /* Zbog nemanja Repository pattern, nema ime tabele, LoadAsync<Product> nadje tabelu Product tipa 
             i odatle izvuce jednu vrstu na osnovu command.Id. */
            if (product == null)
                throw new ProductNotFoundException(command.Id); // Def u Catalog/Exceptions

            // Nikad ne azuriram Id, a sve ostala polja smem da azuriram ako zelim
            product.Name = command.Name;
            product.Category = command.Category;
            product.Description = command.Description;
            product.ImageFile = command.ImageFile;
            product.Price = command.Price;
            /* Zbog NoSQL baze, nema ime tabele, ali Update(product) zna na osnovu typeof(product) = Product
             da mora da azurira tabelu Product tipa. */
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
    }
}
