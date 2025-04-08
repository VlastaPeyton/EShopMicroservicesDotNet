
namespace Ordering.Application.DTOs
{   /* U entity klasi Order.cs, Address polje je tipa Address.cs koje ima vise polja i 
     zato sva polja iz Address mapiram u AddresDTO.*/
    public record AddressDTO(
        string FirstName,
        string LastName,
        string EmailAddress,
        string AddressLine,
        string Country,
        string State,
        string ZipCode); 
    /* Cak je pozeljno a se isto zovu polja u DTO i ValueObject klasi da mapirame automatski bude tj 
     da ne moram da kodiram  mapiranje rucno. */



}
