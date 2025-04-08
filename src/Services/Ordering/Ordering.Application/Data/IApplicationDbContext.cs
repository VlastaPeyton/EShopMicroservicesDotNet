
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Models;

namespace Ordering.Application.Data
{   
    public interface IApplicationDbContext
    {
        /* ApplicationDbContext je definisan u Ordering.Infrastructure layeru, a kao kod Basket trebamo IApplicationDbContext 
         tj interface repository pattern. IApplicationDbContext definisan u Application layer zbog Clean architecture.

          DbContext se koristi, jer Ordering koristi SQL Server (SQL) bazu, a to je pandan IDocumentSession za Marten NoSQL bazu,
        samo sto za razliku od IDocumentSession, ovde mogu definisati ime tabele. Kao IDocumentSession, DbContext
        ima takodje commit i rollback u SaveChangesAsync built-in metodi.

          Kod Basket, imao sam Repository pattern takodje, ali sam imao NoSQL bazi (IDocumentSessionn umesto DbContext), stoga
        u Program.cs nisam imao AddDbContext<BasketDbContext>. Basket ima Vertical slice architecture, pa sam IBasketRepository i
        BasketRepository stavio u isti layer (API layer), dok ovde to nije slucaj.
        */

        DbSet<Customer> Customers { get; }
        DbSet<Order> Orders { get; }
        DbSet<Product> Products { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        /* ApplicationDbContext nasledjuje DbContext koji vec ima SaveChangesAsync metodu, ali deklaracije toga i ovde
         obezbedi Dependency Inversion and decouples Application layer rom Infrastructure layer, allowing me to
         easily replace implementation of DbContext if needed.  Ovo se radi zbog testiranji, jer ako definisem ovu metodu u 
         ApplicationDbContext, onda mogu da testiram bez da koristim bazu. */
    }
}
