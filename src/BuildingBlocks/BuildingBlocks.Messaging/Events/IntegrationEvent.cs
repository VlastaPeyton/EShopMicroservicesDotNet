
namespace BuildingBlocks.Messaging.Events
{   /*Zajednicka struktura za sve Integration Events used in async microservice communication, jer
     Integration Event je Published from microservice1 (Basket) a Subscribed(Consumed) u microservice2
    (Ordering) pomocu RabbitMQ.
      
      Razlika Integration Event i Domain Event, je ta sto Domain Event je Published i Subscribed unutar 
    istog microservice pomocu Mediator, dok Integration event pomocu RabbitMQ.
    */
    public record IntegrationEvent
    {
        /* IntegrationEvent mora imati ista polja kao DomainEvent(OrderCreatedEvent i OrderUpdatedEvent), jer 
         se u Message Broker (RabbitMQ) mapira DomainEvent to IntegrationEvent tj ova klasa je pandan IDomainEvent 
         samo nije interface jer nemamo INotification jer ne koristimo MedaitR nego RabbitMQ. */
        public Guid Id => Guid.NewGuid();
        public DateTime OccuredOn => DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName; // Odnosi se na klasu gde je ovo pozvano

    }
}
