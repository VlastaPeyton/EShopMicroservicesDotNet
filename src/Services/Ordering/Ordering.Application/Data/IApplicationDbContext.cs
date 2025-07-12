
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Models;

namespace Ordering.Application.Data
{   
    public interface IApplicationDbContext
    {
        /* IApplicationDbContext se stavlja u Applicaiton layer. 
          DbContext se koristi, jer Ordering koristi SQL Server (SQL) bazu, a to je pandan IDocumentSession za Marten NoSQL Postgre bazu,
        samo sto za razliku od IDocumentSession, ovde mogu definisati ime tabele. Kao IDocumentSession, DbContext ima takodje commit i rollback u SaveChangesAsync built-in metodi. 
        */

        // Nisam stavio {get;set;}, jer u Applciation DbContext ocu da ovo bute expression-bodied. Eto, malo za promenu. 
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
