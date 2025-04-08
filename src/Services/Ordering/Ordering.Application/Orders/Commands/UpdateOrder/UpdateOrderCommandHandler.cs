

using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.Http;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Exceptions;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom .cs fajlu, zbog Clean architecture.
    public class UpdateOrderCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {   // Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext 
       
        // Mora metoda zbog interface
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {   
            // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            var orderId = OrderId.Of(command.Order.Id);
            
            var order = await dbContext.Orders.FindAsync(orderId, cancellationToken);
            // Da Orders tabela ima 2 PK, moralo bi [polje1, polje2] umesto orderId
            /* Zato sto pretrazuje po orderId (Guid tipe), FindAsync je najbrze nego SingleOrDefaultAsync ili FirstOrDefaultAsync
            i zato pitamo ispod order is null jer nema default ako nista ne nadje nego error baca*/

            if (order is null)
                throw new OrderNotFoundException(command.Order.Id); // Definisao sam 

            // Save to DB
            UpdateOrderWithNewValues(order, command.Order); 
            dbContext.Orders.Update(order); // Buil-in za DbContext
            await dbContext.SaveChangesAsync(cancellationToken); // Ima commit/rollback

            return new UpdateOrderResult(true);
        }

        // Update Order without modify OrderItems list da pozove Update iz Order.cs
        private void UpdateOrderWithNewValues(Order order, OrderDTO orderDto)
        {
            // Update metoda iz Order.cs zahteva  OrderName, ShippingAddress, BillingAddress, Payment, Status

            var updatedShippingAddress = Address.Of(orderDto.ShippingAddress.FirstName,
                                                    orderDto.ShippingAddress.LastName,
                                                    orderDto.ShippingAddress.EmailAddress,
                                                    orderDto.ShippingAddress.AddressLine,
                                                    orderDto.ShippingAddress.Country,
                                                    orderDto.ShippingAddress.State,
                                                    orderDto.ShippingAddress.ZipCode);
            // Mora bas redosledom u Address Of metodi

            var updatedBillingAddress = Address.Of( orderDto.BillingAddress.FirstName,
                                                    orderDto.BillingAddress.LastName,
                                                    orderDto.BillingAddress.EmailAddress,
                                                    orderDto.BillingAddress.AddressLine,
                                                    orderDto.BillingAddress.Country,
                                                    orderDto.BillingAddress.State,
                                                    orderDto.BillingAddress.ZipCode);
            // Mora bas redosledom u Address Of metodi

            var updatedPayment = Payment.Of(orderDto.Payment.CardName,
                                            orderDto.Payment.CardNumber,
                                            orderDto.Payment.Expiration,
                                            orderDto.Payment.Ccv,
                                            order.Payment.PaymentMethod);
            // Mora bas redosledom u Address Of metodi 

            order.Update(orderName: OrderName.Of(orderDto.OrderName),
                         shippingAddress: updatedShippingAddress,
                         billingAddress: updatedBillingAddress,
                         payment: updatedPayment,
                         status: orderDto.Status);
            
            // Update metoda pokrece AddDomainEvent (OrderUpdatedEvent) u DomainEvents listu koju Order.cs nasledio iz Aggregate
        }
    }
}
