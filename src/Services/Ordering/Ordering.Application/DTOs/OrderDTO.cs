

using Ordering.Domain.Enums;

namespace Ordering.Application.DTOs
{
    /* DTO se koristi prilikom writing to DB, da proverimo u Application layeru da li je sve u redu
      sa tim ulaznim podacima, pa ako jeste, onda mapiram DTO u Entity klasu (tabelu) koja je u Domain layeru. 
      Ovim se postize razdvajanje Domain od Application layera i postize se sigurnost proverom ovom. */
    public record OrderDTO( 
        Guid Id, 
        Guid CustomerId,  // Jer Value field of CustomerId je Guid
        string OrderName, 
        AddressDTO ShippingAddress,
        AddressDTO BillingAddress,
        PaymentDTO Payment,
        OrderStatus Status,
        List<OrderItemDTO> OrderItems);
        /* OrderDTO ima sva polja (pa i nasledjena) iz Order.cs, osim TotalPrice, jer to je expression-bodied getter,
         pa ne mogu da ga setujem u DTO klasi.
           Cak je pozeljno da se isto zovu polja u DTO i Entity klasi da mapiranje automatski bude u Mapster tj da ne moram da kodiram 
        mapiranje rucno. Mapiranje je pomocu Mapster library instaliran u BuildingBlocks koga referencira ovaj layer.
           Polje u Order.cs koje jecustom tipa, a taj tip ima samo jedno polje (npr OrderName.cs/Id/CustomerId ima Value polje),
        mapira se u proste tipove koji odgovaraju tipu Value polja zbog JSON serialization.
           Polje u Order.cs koje je custom tipa, a taj tip ima vise polja (npr Address.cs ima vise polja), mapira se u 
        custom DTO tip ovde (AddressDTO).
           Polja koja su UPPERCASE imena, ovde ne smeju biti jer Mapsteru je to problem, ali 
        je on pametan da zna da mapira svakako jer on sve pretvara u upper/lowercase kad mapira. 
           Obzirom da bih morao u Mapsteru namestiti mapiranje polja koja su custom type (AddressDTO, PaymentDTO, OrderItems), radicu rucno mapiranje jer mi lakse.*/


    
    
}
