namespace Basket.API.DTOs
{
    /* Sva ista polja kao BasketCheckoutEvent in BuildingBlocks.Messaging mora zbog mapiranja
    jer pravim DTO za BasketcheckoutEvent, posto Subscriber na RabbitMQ je Ordering koji ima Clean 
    architecture pa da razdvojim API/Applicaiton layer od Domain. DTO sluzi da u Response/Request object ne moze da stoji Event bas jer to niej dobra praksa. 
    DTO objekat ne sme imati custom type polja, jer JSON to ne znaju da protumace kad serialization radi. Npr CustomerId je u Order tipa CustomerID Value object, ali ovde mora biti Guid 
    jer Value iz CustomerId.cs je Guid. Tacnije, ne mora ovako, ali je dobra praksa.! 
    */
    public class BasketCheckoutDTO
    {
        // Polja moraju biti primitive types (not custom Value Object type) zbog JSON Serialization/Deserialization.  

        public string UserName { get; set; } = default!; // Iz ShoppingCart.cs
        public decimal TotalPrice { get; set; } = default!; // Iz ShoppingCart.cs 
        public Guid CustomerId { get; set; } = default!; // Iz Order.cs

        // Shipping/BillingAddress moraju imati ista imena i tip polja kao u Address.cs ValueObject u Ordering zbog mapiranja jer zbog JSON ne moze Address.cs polje ovde
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;

        // Payment mora imati ista imena i tip polja kao u Payment.cs Vlaue Object u Ordering zbog mapiranja jer zbog JSON ne moze Payment.cs ovde
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiration { get; set; } = default!;
        public string CCV { get; set; } = default!;
        public int PaymentMethod { get; set; } = default!;
    }
}
