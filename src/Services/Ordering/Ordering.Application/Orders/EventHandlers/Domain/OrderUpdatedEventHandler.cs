
using MediatR;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain
{
    /* OrderUpdatedEvent je implementirao IDomainEvent koji implementirao INotification, a 
   INotification je iz MediatR, a OrderUpdatedEventHandler : INotificationHandler, a 
   INotificiationHandler je iz MediatR i zato onda moze da handluje event. 

      DispatchDomainEvent.cs ima await mediator.Publish(domainEvent) koji trigeruje INotification. 

      Event Handler samo za Command tj writing to DB se radi. */
    public class OrderUpdatedEventHandler : INotificationHandler<OrderUpdatedEvent>
    {
        // Mora metoda zbog interface 
        public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
