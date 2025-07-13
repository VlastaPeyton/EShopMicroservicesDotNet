
using MediatR;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain
{
    /* Client aktivira UpdateOrderCommandEndpoint koji aktivira UpdateOrderCommandHandler koji modifikuje bazu OrderUpdatedEvent : IDomainEvent koji implementirao INotification, a 
   INotification je iz MediatR, a OrderUpdatedEventHandler : INotificationHandler, a 
   INotificiationHandler je iz MediatR i zato onda moze da handluje event automatski kad u DispatchDomainEventInterceptor mediator.Publish(domainEvent)

      Event Handler samo za Command tj writing to DB se radi. */
    public class OrderUpdatedEventHandler : INotificationHandler<OrderUpdatedEvent>
    {
        // Mora metoda zbog interface 
        public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask; // Posto sam resio da je ovde kraj ciklusa za OrderUpdatedEvent, mram da vratim ovo jer ne moze bez nista
        }
    }
}
