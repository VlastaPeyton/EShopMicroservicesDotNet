

using System.Runtime.ConstrainedExecution;
using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.Http;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Exceptions;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{   // Objasnjeno u CreateOrder
    public class UpdateOrderCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {   
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {   
            var orderId = OrderId.Of(command.Order.Id);
            
            var order = await dbContext.Orders.FindAsync(orderId, cancellationToken);
            /*FindAsync pretrazuje po PK, a Id polje je PK koje, zbog OrderId tipa, u OnModelCreating tj u OrderConfiguration.cs je moralo namesteti se exlicitno.
             Zato sto pretrazuje po orderId(Guid tipe), FindAsync je najbrze nego SingleOrDefaultAsync ili FirstOrDefaultAsync

            Nisam smeo koristiti AsNoTracking, jer mi treba Change Tracking za order, obzirom da SaveChangesAsync nakon Update(order) zahteva chage tracking da bi zamenilo u bazi. */

            if (order is null)
                throw new OrderNotFoundException(command.Order.Id); // Definisao sam 

            // Save to DB
            UpdateOrderWithNewValues(order, command.Order); // order bice izmenjen, jer je reference type 
            dbContext.Orders.Update(order); // Buil-in za DbContext i nema await za Update. Update ce da aktivira DispatchDomainEventInterceptor jer on nasledio SavingChangesInterceptor 
            await dbContext.SaveChangesAsync(cancellationToken); // Ima commit/rollback

            return new UpdateOrderResult(true);
        }

        // Update Order without modify OrderItems list tj koristim Update iz Order.cs koja ne azurira OrderItems
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

            /*Update metoda pokrece AddDomainEvent da doda OrderUpdatedEvenet u DomainEvents polje (koje Order nasledio iz Aggregate),
             a posto OrderUpdatedEvenet implements IDomainEvent (:INotification), automatski ce DispatchDomainEventsInterceptor (zbog IMediator) da 
            publish(OrderUpdatedEvenet) i automatski ce da se pozove OrderUpdatedEvenetHandler (zbog OrderUpdatedEvenetHandler : INotificationHandler<OrderUpdatedEvenet>).*/
        }
    }
}
