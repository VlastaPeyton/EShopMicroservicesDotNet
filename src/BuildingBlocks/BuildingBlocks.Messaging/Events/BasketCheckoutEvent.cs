
namespace BuildingBlocks.Messaging.Events
{
    public record BasketCheckoutEvent : IntegrationEvent
    {   /* Kao sto OrderCreatedEvent i OrderUpdatedEvent nasledili IDomainEvent, tako ovde
          sam nasledio IntegrationEvent */

        /* Polja moraju biti primitive types (not custom) zbog JSON Serialization/Deserialization. 
         Npr ne smem koristi CustomerId jer je custom type. 
           
           Sva polja su public {get;set;} znaci da ih setovati mogu i nakon kreiranja objekta. */

        public string UserName { get; set; } = default!; // Iz ShoppingCart.cs
        public decimal TotalPrice { get; set; } = default!; // Iz ShoppingCart.cs 
        public Guid CustomerId { get; set; } = default!; // Iz Order.cs
        
        
        // Shipping/BillingAddress moraju imati ista imena i tip polja kao u Address.cs ValueObject zbog mapiranja
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine {  get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;

        // Payment mora imati ista imena i tip polja kao u Payment.cs Vlaue Object zbog mapiranja
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiration {  get; set; } = default!;
        public string CCV { get; set; } = default!;
        public int PaymentMethod { get; set; } = default!;
    }
}
