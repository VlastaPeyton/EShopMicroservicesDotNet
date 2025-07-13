namespace Basket.API.DTOs
{   
    // Zbog writing to DTO prvo da proveri ovaj DTO pa ako jeste onda ga mapira u Domain klasu 
    public class StoreShoppingCartDTO
    {   // Ovde Mapster mapira Items bez problema, ako StoreShoppingCartItemDTO ima ista imena polja kao ShoppingCartItem i sva su istog tipa i tip nije custom. Moze i ako je custom, ali onda mora konfigurisem mapster ili rucno.
        public string UserName { get; set; } = default!;
        public List<StoreShoppingCartItemDTO> Items { get; set; } = new();
    }
}
