namespace Basket.API.Models
{   
    // Objasnjeno u ShoppingCartItem
    public class ShoppingCart 
    {   // Posto neam Id polje koje bi automatski bil PK, u Program.cs definisacu UserName da bude PK.
        public string UserName { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = new();
        // Zbog object(reference) type, ovo je Navigational attribute,  ALI ZA NoSQL DB sto omogucava (za razliku od SQL DB) da imamo Items polje ("kolonu" u bazi) u documentu i da nemamo PK-FK jer ovo je Document (NoSQL) DB.
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        // Readonly propery (no set, only get). Zbog => ovo je expression-bodied property cija se vrednost izracuna samo kada mu pristupamo

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }
        // Required for mapping
        public ShoppingCart() { } 
    }

}
