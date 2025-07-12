
namespace Ordering.Application.DTOs
{   /* U entity klasi Order.cs, Address polje je tipa Address.cs koje ima vise polja i 
     zato sva polja iz Address mapiram u AddresDTO. Cak je pozeljno da se isto zovu polja u DTO i ValueObject klasi da mapirame automatski bude tj da ne moram da kodiram mapiranje rucno. 
    DTO sluzi jer client ne sme da posalje i primi argument tipa Domain klase, vec DTO. Ovime se postize separaiton of concern izmedju Application i Domain layera.
    */
    public record AddressDTO(
        string FirstName,
        string LastName,
        string EmailAddress,
        string AddressLine,
        string Country,
        string State,
        string ZipCode
    ); 



}
