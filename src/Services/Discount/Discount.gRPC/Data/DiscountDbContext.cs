using Discount.gRPC.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Data
{   
    // Data Access layer for DB operations
    public class DiscountDbContext : DbContext 
    {  /* DbContext se koristi jer Discount koristi SQL (SQLite) bazu, a to je 
        pandan IDocumentSession za Marten NoSQL bazu, samo sto za razliku od 
        IDocumentSession, ovde mogu definisati ime tabele. Kao IDocumentSession, DbContext
        ima takodje commit i rollback u SaveChangesAsync built-in metodi. */

        // U bazi, tabela ce se zvati Coupons i kao kolone imace polja iz Coupon.cs
        public DbSet<Coupon> Coupons { get; set; } = default!; // Moze da se setuje i van klase ovo polje
        // Konstruktor mora uvek 
        public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options) { }

        /* Znam iz CollegeApp, da ova metoda se sama ponudi kad krenem da je kucam. 
         Ona Seeduje initial data u discountdb bazu (Coupons tabelu) i da dodam uslove za zeljene kolone
        u tabeli ako zelim. Seeduje se samo za development fazu, bas kao kod Catalog sto sam radio.*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {       
            modelBuilder.Entity<Coupon>().HasData(
                /* U Catalog/Data/CatalogInitialData.cs sam Seedovao 5 producta, a ovde moram navesti neke od njih i
                Ovako inicijalizujem objekat Coupon klase jer nema konstruktor. */
                new Coupon { Id = 1, ProductName = "IPhone X", Description = "IPhone X description", Amount = 150},
                new Coupon { Id = 2, ProductName = "Samsung 10", Description = "Samsung 10 description", Amount = 200 }
            );
            /* Da bih Auto-Migrate bazu samo u development (nikad u production), jer kad docker-compose pokrenem, 
             necu moci da Package Manager Console da radim Add-Migrate i Update-Database. Zato dodam Extensions.cs klasu 
            u Discount service, koja radi Auto Migrate, a onda u Program.cs dodam app.UseMigration() da to registrujem. */
        }
    }
}
