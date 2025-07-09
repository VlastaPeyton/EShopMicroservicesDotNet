namespace Discount.gRPC.Models
{   
    // Models folder je Domain layer u kom stoje Entity tabele jer koristim SQLite 

    /* Coupon je vrsta(Entity tabela) u SQL (SQLite u nasem slucaju) bazi. Tabela imace kolone iz Coupon*/
    public class Coupon 
    {
        public int Id { get; set; } // EF Core uzme ovo automatski za PK 
        public string ProductName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Amount { get; set; }

        // Sva polja su piblic set pa mogu i van ove klase da ih setujem
    }
}
