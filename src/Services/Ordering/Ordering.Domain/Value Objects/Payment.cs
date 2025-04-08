
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

        /* Polja nemaju set, tj imaju internal set, pa moram unutar ove klase u konstruktoru
        ili u static metodu da setujem.
          Zbog Rich-domain, koristim static "Of" metodu jer ovo je Value Object.*/

        private Payment() { } // AKo ne stavim ovo migracije nece hteti
        // Jer ktor bez parametra dozvoljava EF core da kreira instances of the Payment class during mapping

        // Mora zbog Of metode i zato je private 
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
            /*Validacija, ali ne moze ona iz BuldingBlocks (MediatR FluentValidation)  jer ovo je
             Domain layer koji ga ne referncira plus ovo nije Endpoint. */

            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(ccv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(ccv.Length, 3);

            return new Payment(cardName, cardNumber, expiration, ccv, paymentMethod);
        }
    }
}
