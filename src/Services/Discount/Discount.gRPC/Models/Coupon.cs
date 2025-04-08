namespace Discount.gRPC.Models
{   
    // Models folder je Domain layer u kom stoje Entity tabele 

    /* Coupon je vrsta(Entity tabela) u SQL (SQLite u nasem slucaju) bazi.
      Kao kod Catalog i Basket, tabela u bazi, bez obzira na SQL/NoSQL, imace 
    kolone iz ove klase. */
    public class Coupon
    {
        public int Id { get; set; } // PK in tabela msm da nisam namestio explicitno nigde ali da kod zna zbog Id da provali da je to PK
        public string ProductName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Amount { get; set; }

        // Sva polja su piblic set pa mogu i van ove klase da ih setujem
    }
}
