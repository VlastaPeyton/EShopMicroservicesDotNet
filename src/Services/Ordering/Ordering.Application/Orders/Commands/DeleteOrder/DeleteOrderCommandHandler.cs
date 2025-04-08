
using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{   // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom .cs fajlu, zbog Clean architecture.
    public class DeleteOrderCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {  // Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext 
       
        // Mora metoda zbog interface
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {  // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            var orderId = OrderId.Of(command.OrderId); // typeof(orderId) = OrderId 

            var order = await dbContext.Orders.FindAsync(orderId, cancellationToken);
            /* Zato sto pretrazuje po orderId (Guid tipe), FindAsync je najbrze nego SingleOrDefaultAsync ili FirstOrDefaultAsync
             i zato pitamo ispod order is null jer nema default ako nista ne nadje nego error baca*/

            if (order is null)
                throw new OrderNotFoundException(command.OrderId);

            // Save to DB
            dbContext.Orders.Remove(order); // Remove je built-in 
            await dbContext.SaveChangesAsync(cancellationToken); // Ima commit/rollback u sebi

            return new DeleteOrderResult(true);
        }
    }
}
