
using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{   // Objasnjeno u CreateOrder
    public class DeleteOrderCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {  
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {  
            var orderId = OrderId.Of(command.OrderId); // typeof(orderId) = OrderId 

            var order = await dbContext.Orders.FindAsync(orderId, cancellationToken);
            /* FindAsync pretrazuje po PK, a Id polje je PK koje, zbog OrderId tipa, u OnModelCreating tj u OrderConfiguration.cs je moralo namesteti se exlicitno.
             Zato sto pretrazuje po orderId (Guid tipe), FindAsync je najbrze nego SingleOrDefaultAsync ili FirstOrDefaultAsync
             
            Nisam smeo koristiti AsNoTracking, jer mi treba Change Tracking za order, obzirom da SaveChangesAsync nakon Remove(order) zahteva chage tracking da bi zamenilo u bazi. */

            if (order is null)
                throw new OrderNotFoundException(command.OrderId);

            // Save to DB
            dbContext.Orders.Remove(order); 
            await dbContext.SaveChangesAsync(cancellationToken); 

            return new DeleteOrderResult(true);
        }
    }
}
