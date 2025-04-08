

namespace Ordering.Application.DTOs
{   /* U entity klasi Order.cs, Payment polje je tipa Payment.cs koje ima vise polja i 
     zato sva polja iz Payment mapiram u PaymentDTO.*/
    public record PaymentDTO(
        string CardName,
        string CardNumber,
        string Expiration,
        string Ccv,
        int PaymentMethond);
    /*Sva polje isto se zovu ko i u Payment.cs, osim Ccv koje je u Payment.cs CCV, ali ne moze CCV ovde
     jer Mapster ne moze da mapira uppercase ime polja, ali Mapster je pametan da zna da CCV <-> Ccv */
}
