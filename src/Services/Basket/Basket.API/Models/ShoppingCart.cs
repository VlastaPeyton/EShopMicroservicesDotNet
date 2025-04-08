namespace Basket.API.Models
{
    public class ShoppingCart // Entity tabela u Postgre NoSQL
    {
        public string UserName { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = new();
        // new() = new List<ShoppingCartItem>()
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        /* Readonly propery (no set, only get). Zbog => ovo je expression-bodied 
         property cija se vrednost izracuna samo kada mu pristupamo. */

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }
        // Required for mapping
        public ShoppingCart() { } 

        // Sva polja su public set, pa mogu i van ove klase da ih setujem
    }

}
