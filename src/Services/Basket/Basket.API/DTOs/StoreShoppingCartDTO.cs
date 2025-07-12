namespace Basket.API.DTOs
{   
    // Zbog writing to DTO prvo da proveri ovaj DTO pa ako jeste onda ga mapira u Domain klasu 
    public class StoreShoppingCartDTO
    {   
        public string UserName { get; set; } = default!;
        public List<StoreShoppingCartItemDTO> Items { get; set; } = new();
    }
}
