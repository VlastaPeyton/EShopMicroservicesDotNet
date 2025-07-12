namespace Basket.API.DTOs
{   
    // Jer ShoppingCart sadrzi ShoppingCartItem tako ReturnShoppingCartDTO sadrzi ovu klasu
    public class ReturnShoppingCartItemDTO
    {   
        // Uzeo sam sve iz ShoppingCart jer mi sve treba , osim ProductId jer to FE ne zanima
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string ProductName { get; set; } = default!;
    }
}
