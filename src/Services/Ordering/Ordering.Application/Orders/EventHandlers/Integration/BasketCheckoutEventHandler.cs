
using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{   /* OrderCreatedEvent flow (integration leading to domain event): User aktivira CheckoutBasketEndpoint (koji sadrzi BasketCheckoutDTO) koji ce aktivirati CheckoutBasketCommandHandler koji ce (zbog IPublishEndpoint) koji ce da mapira BasketCheckoutDTO u BasketCheckoutEvent i  publishEndpoint.Publish(BasketCheckoutEvent), a taj
                                publishing ce da posalje BasketCheckoutEvent u RabbitMQ jer BasketCheckoutEvent: INtegrationEvent, a IntegrationEvent mora imati ista polja kao IDomainEvent jer se u RabbitMQ mapiraju. Obzirom da je Ordering.Application Consumer(Subscribed) na RabbitMQ i u MassTransit u BB je namesteno AddConsumer koje se odnosi 
                                samo na Ordering service, a BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent> i automatski se aktivira kad u RabbitMQ stigne BasketCheckoutEvent. BasketCheckoutEventHandler, mapira BasketCheckoutEvent u CreateOrderCommand kako bi, pomocu ISender, 
                                aktivirao CreateOrderCommandHandler automatski koji ce da pokrene Create metod iz Order koji ce dodati OrderCreatedEvent u DomainEvents listu , a onda dbContext.Orders.Add(order) koji ce aktivirati DispatchDomainEventInterceptor (zbog :SavingChangesInterceptor) koji ce da pokrene mediator.Publish(domainEvent) (IDomainEvent:INotification), 
                                a taj publishing ce da aktivira OrderCreatedEventHanlder automatski (zbog :INotificationEventHandler<OrderCreatedEvent>) i  onda OrderCreatedEventHandler salje OrderCreatedIntegrationEvent u RabbitMQ ako je OrderFullfilment feature ON
     */
    public class BasketCheckoutEventHandler(ISender sender) : IConsumer<BasketCheckoutEvent>
    {   // Ordering je Subscriber(Consumer) na RabbitMQ za BasketCheckoutEvent tj IntegrationEvent. IConsumer je iz MassTransit i zato se ovo automatski aktivira kad u RabbitMQ stigne BasketCheckoutEvent

        // Mora metoda zbog interface, ali stavim da bud async
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {   
            // Create new Order and Order Fullfilment process
            var command = MapToCreateOrderCommand(context.Message); // context.Message = BasketCheckoutEvent 
            // typeof(command) = CreateOrderCommand

            await sender.Send(command); // Mapira BasketCheckoutEvent u CreateOrderCommand jer ovime automatski aktivira CreateOrderCommandHandler 
        }

        // BasketCheckoutEvent nisam preko Mapster mogo direktno u CreateOrderCommand, jer CreateOrderCommand sadrzi polje custom typa OrderDTO pa moram rucno
        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            var shippingAddressDTO = new AddressDTO(message.FirstName,
                                            message.LastName,
                                            message.EmailAddress,
                                            message.AddressLine,
                                            message.Country,
                                            message.State,
                                            message.ZipCode);
            // Mora ovim redosledom zbog AddressDTO 
            var billingAddressDTO = new AddressDTO(message.FirstName,
                                            message.LastName,
                                            message.EmailAddress,
                                            message.AddressLine,
                                            message.Country,
                                            message.State,
                                            message.ZipCode);
            // Mora ovim redosledom zbog AddressDTO 
            var paymentDTO = new PaymentDTO(message.CardName,
                                            message.CardNumber,
                                            message.Expiration,
                                            message.CCV,
                                            message.PaymentMethod);
            // Mora ovim redosledom zbog PaymentDTO 

            var orderId = Guid.NewGuid(); // Ne moze = OrderId.Of(Guid.NewGuid()) jer Id polje u OrderDTO je Gudi, a ne OrderId tipa !!! 
                                           
            var orderDTO = new OrderDTO(Id: orderId,
                                        CustomerId: message.CustomerId,
                                        OrderName: message.UserName,
                                        ShippingAddress: shippingAddressDTO,
                                        BillingAddress: billingAddressDTO,
                                        Payment: paymentDTO,
                                        Status: Ordering.Domain.Enums.OrderStatus.Pending,
                                        OrderItems: [
                                            /*Stavicu hardcoced vrednoti iz Products tabele tj iz InitialData.cs ProductsInitialData metode
                                            za ProductId i Price, jer svaki OrderItem je Product iz Producst tabele, ali u praksi nije hardcoded naravno. 
                                              
                                              U InitialData.cs sam objasnio da CatalogInitialData.cs nema veze sa Ordering, jer se
                                            to koristi za Seeding of catalogdb kad Basket testiram. 

                                              Seedovao sam Products tabelu i ostale tabele u Ordering jer to trebaju nam te vrednosti 
                                              
                                              Svaki OrderItem(OrderItemDTO) mora imati isti orderId jer istom Orderu pripadaju, dok 
                                            ProductId i Price su iz Products tabele Seedovane vrednosti from InitialData.cs ProductsInitialData metode */
                                            new OrderItemDTO(orderId, new Guid("e5932196-713a-49c4-9f9a-6bb92304cccf"), 2, 500), // IPhone X
                                            new OrderItemDTO(orderId, new Guid("21a90a06-7908-4fd9-9cbd-12d9c025a51b"), 1, 400)] // Samsung 10
                                        );

            return new CreateOrderCommand(orderDTO);
        }
    }
}
