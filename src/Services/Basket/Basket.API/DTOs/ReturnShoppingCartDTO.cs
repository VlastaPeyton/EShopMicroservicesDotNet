﻿namespace Basket.API.DTOs
{   
    // Razdvaja Domain (Models folder) od Applicaiton i API layera pa moram da mapiram sta mi sve treba iz ShoppingCart u ovaj DTO jer to je dobra praksa da clientu se ne salje Domain object.
    public class ReturnShoppingCartDTO
    {   // Uzimam iz Shoppingcart polja koja mi trebaju da vratim klijentu
        // Moze Mapster da mapira Items from ShoppingCart ako ReturnShoppingCartItemDTO i ShoppingCartItem imaju ista imena i tipove polja ali da nije custom type. ako je custom type onda mapster mora se configurise ili rucno mapiranje
        public string UserName { get; set; } = default!;
        public List<ReturnShoppingCartItemDTO> Items { get; set; } // Mora i navigational attribute da ima DTO klasu a ne Domain klasu
        public decimal TotalPrice { get; set; } // U ShoppingCart ovo je expression-bodied getter ali ovde ne moze to jer moram da imam set zbog slanja klijentu
    }
}
