using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Domain.Models;

namespace Ordering.Infrastructure.Data
{
    // DbContext sluzi za definisanje imena tabela, a u OnModelCreating pravim PK, FK, PK-FK relacije, Seedujem tabelu ako treba, definisem uslove za zeljene kolone tabele i Indexiranje tabele ako treba.
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {  /* U DependencyInjection.cs za Infrastrustrucure layer, uradicu "builder.Services.AddDbContext<ApplicationDbContext>(...)" 
        i "services.AddScoped<IApplicationDbContext, ApplicationDbContext>()", jer radim sa Repository pattern da bi se IApplicationDbContext registrovao kao ApplicationDbContext.   
          
          Necu koristiti Repository za Ordering.
           
          IApplicationDbContext je definisan u Ordering.Application layeru zbog Clean architecture.

          DbContext se koristi,jer Ordering koristi SQL Server (SQL) bazu, a to je pandan IDocumentSession za Marten NoSQL Postgre, samo sto za razliku od IDocumentSession, ovde mogu definisati ime tabele. 
        Kao IDocumentSession, DbContext ima takodje commit i rollback u SaveChangesAsync built-in metodi.
        */

        // Kod Discount svaka tabela imala je {get;set;}, dok ovde ce imati samo expression-bodied getter, jer sam u IApplicationDbContext samo {get;} stavio 
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   /* U CollegeApp imao sam definisanje uslova za zeljene kolone Customers tabele (modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired().HasMaxLength(100)) ali tako necu raditi ovde, jer morao bih za sve 4 tabele da definisem uslove za zeljene kolone i onda
            bi OnModelCreating bio ogromno. Vec cu da napravim CustomerConfiguration.cs, ProductConfiguration.cs, OrderConfiguration.cs i OrderItemConfiguration.cs gde cu definisati uslove za zeljene kolone svake tabele i PK-FK relacije.
            Seeding cu da napisem u DatabaseExtension jer ne zelim da stoji u Configuration klasama. */
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Na osnovu u kojoj Configuration.cs se nalazim, ona se izvrsava.
        }

    }
}
