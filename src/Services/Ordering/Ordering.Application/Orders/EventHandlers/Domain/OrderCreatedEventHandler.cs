
using MassTransit;
using MediatR;
using Microsoft.FeatureManagement;
using Ordering.Application.Extensions;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain
{
    /* OrderCreatedEvent je implementirao IDomainEvent koji implementirao INotification, a 
    INotification je iz MediatR, a OrderCreatedEventHandler : INotificationHandler, a 
    INotificiationHandler je iz MediatR i zato onda moze da handluje event. 
    
       DispatchDomainEvent.cs ima await mediator.Publish(domainEvent) koji trigeruje INotification. a onda se 
    automatski OrderCreatedEvenHandler aktivira cim se u DomainEvents upise OrderCreatedEvent.
    
       Event Handler samo za Command tj writing to DB se radi.  

    U Basket, CheckoutBasketCommandHandler ce da publishEndpoint.Publish(BasketCheckoutEvent) u RabbitMQ kada Client u frontend ide na checkout.
    Ordering je Subscriber za to na RabbitMQ i zbog BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>  ce u BasketCheckoutEventHandler da 
    se aktivira Consume metoda  koja ce da preko MediatR da prosledi CreateOrderCommand u  CreateOrderCommanHandler, a CreateOrderCommanHandler
    pokrece CreateNewOrder metodu koja pokrece Order.Create(iz Order.cs), a  Order.Create metoda ce pozvati AddDomainEvent da doda OrderCreatedEvent 
    u DomainEvents polje (koje Order nasledio iz Aggregate), a posto OrderCreatedEvent implements IDomainEvent (:INotification), automatski ce
    DispatchDomainEventsInterceptor (zbog IMediator) ce da uradi publish(OrderCreatedEvent), a OrderCreatedEventHandler 
    (zbog OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>) ce da Handle OrderCreatedEvent tj da Publish 
    OrderCreatedIntegrationEvent  u RabbitMQ (prvi put da Ordering je Publisher u RabbitMQ). Zbog OrderCreatedEvent 
    aktivira se DispatchDomainEventsInterceptor. */
    public class OrderCreatedEventHandler(IPublishEndpoint publishEndpoint, IFeatureManager featureManager) : INotificationHandler<OrderCreatedEvent>
    {   // IPublishEndpoint je pandan IMediator iz DispatchDomainEventsInterceptor 

        // Mora metoda zbog interface i dodajem async 
        public async Task Handle(OrderCreatedEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            /* Nije dobra praksa to publish IntegrationEvent from DomainEvent Handler na ovaj nacin, jer kada se 
            kreira new Order iz Ordering CreateOrderEndpoint, ova metoda ce da se poziva automatski zbog INotificationHandler<OrderCreatedEvent>
            i IntegrationEvent ce da ide u RabbitMQ bespotrebno. Umesto ovoga, treba CreateOrderIntegrationEvent ici u RabbitMQ 
            samo kad se Subscriber(Order Fullfilment event) radi. 
              Takodje, kada Ordering microservice start up, radi se Migration i Seedin automatski i onda ce da se pokrene ovaj OrderCreatedEventHandler 
            koji ce da publish OrderCreatedIntegrationEvent u RabbitMQ dzebe. 

              Ovo cemo  srediti pomocu Feature Management tako sto proveravamo dok application running bez da je gasimo. */
            if (await featureManager.IsEnabledAsync("OrderFullfilment"))
            {
                // First, map incoming OrderCreatedEvent to orderDTO
                var OrderCreatedIntegrationEvent = orderCreatedDomainEvent.Order.ToOrderDto();
                // ToOrderDto je extension method u OrderExtensions.cs 

                // Publish OrderCreatedIntegraationEvent to RabbitMQ
                await publishEndpoint.Publish(OrderCreatedIntegrationEvent, cancellationToken);
            }

            /*Sada, kreiranjem new order with CreateOrderEndpoint ili Seedin Ordering DB on start up, necemo OrderCreatedIntegrationEvent publish
             ako OrderFullfilment nije ON. */
        }
    }
}
