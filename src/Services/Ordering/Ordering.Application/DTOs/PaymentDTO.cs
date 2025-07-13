

namespace Ordering.Application.DTOs
{   // Objasnjeno u OrderDTO
    public record PaymentDTO(
        string CardName,
        string CardNumber,
        string Expiration,
        string Ccv, // Ne moze CCV kao u Payment.cs, jer Mapster ne podrzava all uppercase, ali auotmatski pretvara CCV<->Ccv
        int PaymentMethond);
}
