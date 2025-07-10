
namespace Ordering.Domain.Value_Objects
{   
    // Value object
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get; } = default!;
        public string CCV { get; } = default!;
        public int PaymentMethod { get; } = default!;

        /* Polja nemaju set, tj imaju private set, pa moram unutar ove klase u konstruktoru ili u static metodu da setujem.
          Zbog Rich-domain, koristim static "Of" metodu za Value Object umensto konstruktora.*/
        private Payment() { } // Ako ne stavim ovo migracije nece hteti, jer konstruktor  bez parametra dozvoljava EF core da kreira instances of the Payment class during mapping

        // Polja nemaju explicitni "private set" => ne moze new Address {....} u Of metodi, vec mora private konstruktor koji cu koristiti u Of metodi
        private Payment(string cardName, string cardNumber, string expiration, string ccv, int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CCV = ccv;
            PaymentMethod = paymentMethod;
        }

        public static Payment Of(string cardName, string cardNumber, string expiration, string ccv, int paymentMethod)
        {
            ///Validacija se radi ovde jer Value Object tj custom type, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je Domain layer koji ga ne referncira plus ovo nije Endpoint. 

            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(ccv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(ccv.Length, 3);

            return new Payment(cardName, cardNumber, expiration, ccv, paymentMethod);
        }
    }
}
