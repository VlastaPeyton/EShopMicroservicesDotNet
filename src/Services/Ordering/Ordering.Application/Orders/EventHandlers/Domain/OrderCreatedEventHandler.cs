
using MassTransit;
using MediatR;
using Microsoft.FeatureManagement;
using Ordering.Application.Extensions;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain
{
    /*OrderCreatedEvent flow (integration leading to domain event): User aktivira CheckoutBasketEndpoint (koji sadrzi BasketCheckoutDTO) koji ce aktivirati CheckoutBasketCommandHandler koji ce (zbog IPublishEndpoint) koji ce da mapira BasketCheckoutDTO u BasketCheckoutEvent i  publishEndpoint.Publish(BasketCheckoutEvent), a taj
                                publishing ce da posalje BasketCheckoutEvent u RabbitMQ jer BasketCheckoutEvent: INtegrationEvent, a IntegrationEvent mora imati ista polja kao IDomainEvent jer se u RabbitMQ mapiraju. Obzirom da je Ordering.Application Consumer(Subscribed) na RabbitMQ i u MassTransit u BB je namesteno AddConsumer koje se odnosi 
                                samo na Ordering service, a BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent> i automatski se aktivira kad u RabbitMQ stigne BasketCheckoutEvent. BasketCheckoutEventHandler, mapira BasketCheckoutEvent u CreateOrderCommand kako bi, pomocu ISender, 
                                aktivirao CreateOrderCommandHandler automatski koji ce da pokrene Create metod iz Order koji ce dodati OrderCreatedEvent u DomainEvents listu , a onda dbContext.Orders.Add(order) koji ce aktivirati DispatchDomainEventInterceptor (zbog :SavingChangesInterceptor) koji ce da pokrene mediator.Publish(domainEvent) (IDomainEvent:INotification), 
                                a taj publishing ce da aktivira OrderCreatedEventHanlder automatski (zbog :INotificationEventHandler<OrderCreatedEvent>) i onda OrderCreatedEventHandler salje OrderCreatedIntegrationEvent u RabbitMQ ako je OrderFullfilment feature ON*/
    public class OrderCreatedEventHandler(IPublishEndpoint publishEndpoint, IFeatureManager featureManager) : INotificationHandler<OrderCreatedEvent>
    {   // IPublishEndpoint je pandan IMediator iz DispatchDomainEventsInterceptor . Ovo takodje koristi CheckoutBasketCommandHandler kad salje CheckoutBasketEvent u RabbitMQ.

        // Mora metoda zbog interface i dodajem async 
        public async Task Handle(OrderCreatedEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            /* Moram omoguciti IFeatureManager ozbirom da CreateOrderCommandHandler mogu da aktiviram gadjanjem CreateOrderEndpoint ili gadjanjem BasketCheckoutEndpoint. 
             Samo u slucaju BasketCheckout, zelim da se odavde posalje novi integration event u RabbitMQ. U ostalim slucajevima ne zelim. I zato feature manager.

              Ovo cemo  srediti pomocu Feature Management tako sto proveravamo dok application running bez da je gasimo. */
            if (await featureManager.IsEnabledAsync("OrderFullfilment"))
            {
                // First, map incoming OrderCreatedEvent tj njegovo Order polje to orderDTO manuelno jer zbog custom type mapster ne moze ili mora se modifikuje rucno
                var OrderCreatedIntegrationEvent = orderCreatedDomainEvent.Order.ToOrderDto();

                // Publish OrderCreatedIntegraationEvent to RabbitMQ
                await publishEndpoint.Publish(OrderCreatedIntegrationEvent, cancellationToken);
                // Posto OrderCreatedIntegrationEvent ima custom type polja, u nastavku bih morao da rucno mapiram iz OrderCreatedIntegrationEvent u neki DomainEvent
            }

            /*Sada, kreiranjem new order with CreateOrderEndpoint ili Seedin Ordering DB on start up, necemo OrderCreatedIntegrationEvent publish
             ako OrderFullfilment nije ON, vec stavicu ON samo kad gadjam BasketCheckoutCommandHandler. */
        }
    }
}
