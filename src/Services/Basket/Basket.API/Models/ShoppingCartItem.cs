namespace Basket.API.Models
{    // Basket service ima Vertical slice arhitekturu, pa svi layers su unutar istog projekta under Basket folder. Da je Clean architecture, Models folder bi bio Domain layer posebno, a ovde je unutar Catalog foldera Domain layer.
     // Bakset koristi NoSQL bazu (Postgres via Marten,a Marten je pandan EF Core). U NoSQL, Tabela se zove Collection, Row se zove Document, Column se zove Field.
     // NoSQL DB nema PK-FK
    public class ShoppingCartItem // Ovo nije Entity, jer ne radim SQL tj EF Core, vec NoSQL (via Marten), ali ovo svakako predstavlja "tabelu" tipa ShoppingCartItem
    {   // ShoppingCartItem is nested in ShoppingCart, so Marten will only allow PK for ShoppingCart, not for ShoppingCartItem
        public int Quantity { get; set;} = default!;
        // Znam iz Catalog Product.cs sta ovo znaci 
        public string Color { get; set;} = default!;
        public string Name { get; set;} = default!;
        public decimal Price { get; set;} = default!;
        public Guid ProductId { get; set;} = default!; // Trudim se da bude Guid umesto int jer je bolje.
        public string ProductName { get; set;} = default!;

        // Sva polja su public set pa mogu i van ove klase da ih setujem
    }
}
