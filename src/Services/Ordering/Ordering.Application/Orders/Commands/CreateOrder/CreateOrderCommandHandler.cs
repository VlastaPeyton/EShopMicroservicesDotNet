
using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    // CQRS objasnjen u Catalog i Basket. 

    /*OrderCreatedEvent flow (integration leading to domain event): User aktivira CheckoutBasketEndpoint (koji sadrzi BasketCheckoutDTO) koji ce aktivirati CheckoutBasketCommandHandler koji ce (zbog IPublishEndpoint) koji ce da mapira BasketCheckoutDTO u BasketCheckoutEvent i  publishEndpoint.Publish(BasketCheckoutEvent), a taj
                                publishing ce da posalje BasketCheckoutEvent u RabbitMQ jer BasketCheckoutEvent: INtegrationEvent, a IntegrationEvent mora imati ista polja kao IDomainEvent jer se u RabbitMQ mapiraju. Obzirom da je Ordering.Application Consumer(Subscribed) na RabbitMQ i u MassTransit u BB je namesteno AddConsumer koje se odnosi 
                                samo na Ordering service, a BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent> i automatski se aktivira kad u RabbitMQ stigne BasketCheckoutEvent. BasketCheckoutEventHandler, mapira BasketCheckoutEvent u CreateOrderCommand kako bi, pomocu ISender, 
                                aktivirao CreateOrderCommandHandler automatski koji ce da pokrene Create metod iz Order koji ce dodati OrderCreatedEvent u DomainEvents listu , a onda dbContext.Orders.Add(order) koji ce aktivirati DispatchDomainEventInterceptor (zbog :SavingChangesInterceptor) koji ce da pokrene mediator.Publish(domainEvent) (IDomainEvent:INotification), 
                                a taj publishing ce da aktivira OrderCreatedEventHanlder automatski (zbog :INotificationEventHandler<OrderCreatedEvent>) i onda ide handler ovaj*/

    // U OrderCreatedEventHandler dodao IFeatureManagement je ne zelim da se on aktivira ako client direknto gadja CreateOrderEndpoint. Objasnjeno je tamo sve. 

    public class CreateOrderCommandHandler (IApplicationDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {   // Neam Repository, pa zato IApplicationDbContext koristim i logiku pisem ovde. U Infrastructure layer IApplicationDbContext registrovan kao ApplicaitonDbContext.

        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {   // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            var order = CreateNewOrder(command.Order); // typeof(comman.Order) = OrderDTO 
            // typeof(order) = Order

            // Save to DB
            dbContext.Orders.Add(order); // Built-in metoda za DbContext koja ima Change Tracker automatski i onda ce nakon SaveChangesAsync da se promena upise u bazu
            await dbContext.SaveChangesAsync(cancellationToken); // Ima commit/rollback u sebi

            // return result 
            return new CreateOrderResult(order.Id.Value); // Id polje of Order je OrderId Value Object with Value field. Ovo je u OrderConfiguration.cs namesteno how to write/read to DB with value object 
        }

        // Radim rucno mapiranje, jer OrderDTO i Order imaju custom type neka polja i onda da ne bih modifikovao Mapster, lakse mi je rucno da mapiram, jer Mapster zbog custom types zahteva da mu explicitno objasnim
        private Order CreateNewOrder(OrderDTO orderDTO)
        {
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

            var order = Order.Create(
                orderId: OrderId.Of(Guid.NewGuid()),
                customerId: CustomerId.Of(orderDTO.CustomerId),
                orderName: OrderName.Of(orderDTO.OrderName),
                shippingAddress: shippingAddress,
                billingAddress: billingAddress,
                payment: payment
            );
            // Levo od : je ime argumenta u Order.cs Create metodi, moglo je i bez toga, ali je nekad preglednije. Ako vec pisem imena, moram pisati imena bas ista ko u Create metodi.

            /*Create metoda pokrece AddDomainEvent da doda OrderCreatedEvent u DomainEvents polje (koje Order nasledio iz Aggregate),
            a posto OrderCreatedEvent implements IDomainEvent (:INotification), automatski ce DispatchDomainEventsInterceptor (zbog IMediator) da 
            publish(OrderCreatedEvenet) i automatski ce da se pozove OrderCreatedEventHandler (zbog OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>).*/

            // Mapiram OrderItems listu iz OrderDTO u Order.cs jer u Create nema argument za OrderItems
            foreach (var orderItemDTO in orderDTO.OrderItems)
                order.AddOrderItem(ProductId.Of(orderItemDTO.ProductId), orderItemDTO.Quantity, orderItemDTO.Price);

            return order;
        }
    }
}
