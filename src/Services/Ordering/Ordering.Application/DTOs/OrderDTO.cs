

using Ordering.Domain.Enums;

namespace Ordering.Application.DTOs
{   
    /* DTO se koristi prilikom writing to DB, da proverimo u Application layeru da li je sve u redu
     sa tim ulaznim podacima, pa ako jeste, onda mapiram DTO u Entity klasu (tabelu) koja je u Domain layeru. 
     Ovim se postize razdvajanje Domain od Application layera i postize se sigurnost proverom ovom. 
     */
    public record OrderDTO( // DTO for Order.cs entity tj za Orders tabelu
        Guid Id, 
        Guid CustomerId, 
        string OrderName, 
        AddressDTO ShippingAddress,
        AddressDTO BillingAddress,
        PaymentDTO Payment,
        OrderStatus Status,
        List<OrderItemDTO> OrderItems);
        /* OrderDTO ima sva polja (pa i nasledjena) iz Order.cs, osim TotalPrice, jer to je expression-bodied getter,
         pa ne mogu da ga setujem.
           Cak je pozeljno a se isto zovu polja u DTO i Entity klasi da mapirame automatski bude tj da ne moram da kodiram 
        mapiranje kao kod CollegeApp.  Mapiranje je pomocu Mapster library instaliran u BuildingBlocks koga referencira ovaj layer.
           Polja u Order.cs koja su custom tipa, a taj tip ima samo jedno polje (npr OrderName.cs/Id/CustomerId ima Value polje string tipa),
        mapiraju se u proste tipove ovde. 
           Polja u Order.cs koja su custom tipa, a taj tip ima vise polja (npr Address.cs ima vise polja), mapiraju se u 
        custom DTO tipove ovde (AddressDTO).
           Polja koja su u ValueObject ili Entity klasi uppercase imena, ovde ne smeju biti jer Mapsteru je to problem, ali 
        je on pametan da zna da mapira svakako jer on sve pretvara u upper/lowercase kad mapira. */


    
    
}
