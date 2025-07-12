namespace Basket.API.DTOs
{
    public class StoreShoppingCartItemDTO
    {
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public Guid ProductId { get; set; } = default!; // Trudim se da bude Guid umesto int jer je bolje.
        public string ProductName { get; set; } = default!;
    }
}
