namespace Basket.API.Models
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; } = default!;
        // Znam iz Catalog Product.cs sta ovo znaci 
        public string Color { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public Guid ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;

        // Sva polja su publik set pa mogu i van ove klase da ih setujem
    }
}
