namespace Basket.API.DTOs
{
    public class StoreShoppingCartItemDTO
    {   // Sva polja iz ShopingCartItem su primary type (nema Value Object) ,a i ovde su, i onda Mapster mapira to bez problema.
        // Da je neko polje bar jednog od dva objekta Value Object (custom type) Mapster ne bi to mogo, vec morao bih ili rucno ili da modifikujem Mapster
        
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public Guid ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
    }
}
