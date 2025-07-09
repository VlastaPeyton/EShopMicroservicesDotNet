// using sam stavio u global using

namespace Catalog.API.Products.UpdateProduct
{   // Pogledaj CreateProductEndpoint, da ne ponavljam. 
    public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);

    // U Command (nikad Query)  moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske. Ovo je strongly typed validaton from MediatR.
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        // Konstruktor mora ovako zbog nasledjivanja
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is requiered");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category is requiered").Length(2, 150).WithMessage("Name must be [2,150]");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price is >0 mora");
            // Category, Description, Image file nisam hteo da validiram eto tako
        }
    }
    public class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {            
            // Prvo ce se odraditi validacija ( koju sam definisao je u BuildingBlocks + dodao u MediatR pipeline).

            var product = await session.LoadAsync<Product>(command.Id, cancellationToken); // Nema Change Tracker jer koristim LightWeightDocumentSession
            // Moze LoadAsync by Id, jer Id field u Product.cs je PK.
            // Zbog Marten, nema ime tabele, vec LoadAsync<Product> nadje tabelu Product tipa i odatle izvuce jednu vrstu na osnovu command.Id.
            if (product == null)
                throw new ProductNotFoundException(command.Id); // Definisano u Exceptions folderu

            // Nikad ne mogu da azuriram Id, a sve ostala polja smem da azuriram ako zelim.
            product.Name = command.Name;
            product.Category = command.Category;
            product.Description = command.Description;
            product.ImageFile = command.ImageFile;
            product.Price = command.Price;
            /* Zbog NoSQL baze tj Marten , nema ime tabele, ali Update(product) zna na osnovu typeof(product) = Product da mora da azurira tabelu Product tipa. */
            session.Update(product); // Moram manuelno ovo uraditi, jer koristim LightWeightDocumentSession koji nema Change Tracking pa samo SaveChangesAsync da bude dovoljan.
            await session.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
    }
}
