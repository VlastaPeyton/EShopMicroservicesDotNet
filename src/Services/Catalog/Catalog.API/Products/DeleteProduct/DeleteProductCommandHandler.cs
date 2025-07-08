//usings sam stavio u globalusing

using Catalog.API.Products.UpdateProduct;

namespace Catalog.API.Products.DeleteProduct
{   // Command slucaj objasnjen u CreateProductCommandHandler, da se ne ponavljam ovde.

    public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
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
    { 
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            // Prvo se odradi valdicija ( koju definisao u BuildingBlocks i dodao u MediatR pipeline u Program.cs), pa tek Handle.

            session.Delete<Product>(command.Id);
            // Zbog NoSQL baze tj Marten, nema ime tabele, ali Delete<Product> nadje tabelu tipa Product i izbrise zeljenu "vrstu" tj document. 
            
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
