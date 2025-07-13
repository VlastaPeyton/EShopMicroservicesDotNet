namespace Basket.API.DTOs
{   
    // Jer ShoppingCart sadrzi ShoppingCartItem tako ReturnShoppingCartDTO sadrzi ovu klasu
    public class ReturnShoppingCartItemDTO
    {   
        /*Uzeo sam sve iz ShoppingCartItem jer mi sve treba , osim ProductId jer to FE ne zanima. 
        Da je neko od ovih polja u ShoppingCart bilo Value Object, ovde je dobra praksa naciniti ga primary tipom tj tipom of Value field iz tog Value Object .
        Mapster nema problem sto ProductId polje nije ovde uzeto, jer on ce njega preskociti kad mapira iz SCItem to ReturnShoppingCartItemDTO*/
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string ProductName { get; set; } = default!;
    }
}
