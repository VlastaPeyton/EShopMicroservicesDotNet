
using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom .cs fajlu, zbog Clean architecture.

    public class CreateOrderCommandHandler (IApplicationDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {   // Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext 

        // Mora metoda zbog interface
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {   // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            // Create Order entity from command object
            var order = CreateNewOrder(command.Order); // typeof(comman.Order) = OrderDTO 
            // typeof(order) = Order, jer Orders tabela ima Order.cs tip 

            // Save to DB
            dbContext.Orders.Add(order); // Built-in metoda za DbContext 
            await dbContext.SaveChangesAsync(cancellationToken); // Ima commit/rollback u sebi

            // return result 
            return new CreateOrderResult(order.Id.Value);
            /* typeof(order) = Order, a Order.cs nasledio Id iz Aggreage<OrderId> (tj iz Entity<OrderId>), a OrderId ima Value polje Guid tipa.*/
        }

        private Order CreateNewOrder(OrderDTO orderDTO)
        {
            /* Mapiramnje polja from OrderDTO to Order.cs ali custom mapiranje a kasnije cemo automatsko pomocu Mapster. */

            var shippingAddress = Address.Of(orderDTO.ShippingAddress.FirstName,
                                             orderDTO.ShippingAddress.LastName,
                                             orderDTO.ShippingAddress.EmailAddress,
                                             orderDTO.ShippingAddress.AddressLine,
                                             orderDTO.ShippingAddress.Country,
                                             orderDTO.ShippingAddress.State,
                                             orderDTO.ShippingAddress.ZipCode); 
            // Bas ovaj redosled jer tako Address Of metoda zahteva

            var billingAddress = Address.Of(orderDTO.BillingAddress.FirstName,
                                             orderDTO.BillingAddress.LastName,
                                             orderDTO.BillingAddress.EmailAddress,
                                             orderDTO.BillingAddress.AddressLine,
                                             orderDTO.BillingAddress.Country,
                                             orderDTO.BillingAddress.State,
                                             orderDTO.BillingAddress.ZipCode);
            // Bas ovaj redosled jer tako Address Of metoda zahteva

            var payment = Payment.Of(orderDTO.Payment.CardName,
                                     orderDTO.Payment.CardNumber,
                                     orderDTO.Payment.Expiration,
                                     orderDTO.Payment.Ccv,
                                     orderDTO.Payment.PaymentMethond);
            // Bas ovaj redosled jer tako Payment Of metoda zahteva

            var newOrder = Order.Create(
                orderId: OrderId.Of(Guid.NewGuid()),
                customerId: CustomerId.Of(orderDTO.CustomerId),
                orderName: OrderName.Of(orderDTO.OrderName),
                shippingAddress: shippingAddress,
                billingAddress: billingAddress,
                payment: payment);
            /* Nema status u Create metodi jer Enum nismo  prosledjuje kao argument posto i ne mora kad se creira order
            Ovo levo od : je ime argumenta u Order.cs Create metodi, moglo je i bez toga, ali je nekad preglednije.
            Ako vec pisem imena, moram pisati imena bas ista ko u Create metodi.
               
               Create metoda pokrece AddDomainEvent da doda OrderCreatedEvent u DomainEvents polje (koje Order nasledio iz Aggregate),
            a posto OrderCreatedEvent implements IDomainEvent (:INotification), pa ce DispatchDomainEventsInterceptor (zbog IMediator) da 
            publish(OrderCreatedEvenet) i automatski ce da se pozove OrderCreatedEventHandler (zbog OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>) .*/

            // Mapiram OrderItems listu iz OrderDTO u Order.cs jer u Create nema argument za OrderItems
            foreach (var orderItemDTO in orderDTO.OrderItems)
                newOrder.AddOrderItem(productId: ProductId.Of(orderItemDTO.ProductId),
                                      quantity: orderItemDTO.Quantity,
                                      price: orderItemDTO.Price);

            return newOrder;
        }
    }
}
