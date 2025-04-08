
using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{   /* Zaduzen za  kreiranje Order tj CreateOrderCommandHandler.cs poziva, nakon sto, preko RabbitMQ, primi 
     BasketCheckoutEvent (Integration Event), jer kad u Client fronetnd kliknem Checkout 
     sa zeljenim Products(ShoppingCartItem) u ShoppingCart onda se trigeruje Ordering da 
     odradi sve to. */
    public class BasketCheckoutEventHandler(ISender sender) : IConsumer<BasketCheckoutEvent>
    {   // Ordering je Subscriber(Consumer) na RabbitMQ za IntegrationEvent 
        // Mora metoda zbog interface, ali stavim da bud async
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {   
            // Create new Order and Order Fullfilment process
            var command = MapToCreateOrderCommand(context.Message); // context.Message = BasketCheckoutEvent 
            // typeof(command) = CreateOrderCommand

            await sender.Send(command);
            /* U Basket, CheckoutBasketCommandHandler ce da publishEndpoint.Publish(BasketCheckoutEvent) u RabbitMQ kada Client u frontend ide na checkout.
            Ordering je Subscriber za to na RabbitMQ i zbog BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>  ce u BasketCheckoutEventHandler da 
            se aktivira Consume metoda  koja ce da preko MediatR da prosledi CreateOrderCommand u  CreateOrderCommanHandler, a CreateOrderCommanHandler
            pokrece CreateNewOrder metodu koja pokrece Order.Create(iz Order.cs), a  Order.Create metoda ce pozvati AddDomainEvent da doda OrderCreatedEvent 
            u DomainEvents polje (koje Order nasledio iz Aggregate), a posto OrderCreatedEvent implements IDomainEvent (:INotification), automatski ce
            DispatchDomainEventsInterceptor (zbog IMediator) ce da uradi publish(OrderCreatedEvent), a OrderCreatedEventHandler 
            (zbog OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>) ce da Handle OrderCreatedEvent tj da Publish 
            OrderCreatedIntegrationEvent (definisacu kasnije) u RabbitMQ (prvi put da Ordering je Publisher u RabbitMQ). Zbog OrderCreatedEvent 
            aktivira se DispatchDomainEventsInterceptor. Dodacemo Feature Management da bi OrderCreatedEventHandler aktivirao samo kad Basket Checkout radim a 
            ne i za CreateOrderEndpoint ili Seedovanje Ordering baze priliko application start up. */
        }

        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            // Koristim OrderDTO, jer Clean architecture, pa razdvajam Application od Domain layera 

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

            var orderId = Guid.NewGuid(); // OrderId nema Seedovan nikad u InitialData.cs jer to se uvek pravi Guid.NewGuid()

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
                                            to koristi za Seeding of catalogdb da kad Basket testiram. 

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
