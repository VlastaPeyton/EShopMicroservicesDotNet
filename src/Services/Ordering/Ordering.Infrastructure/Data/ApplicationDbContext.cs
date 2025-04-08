using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Domain.Models;

namespace Ordering.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {  /* U DependencyInjection.cs za Infrastrustrucure layer, uradicu "builder.Services.AddDbContext<ApplicationDbContext>(...)" 
        i "services.AddScoped<IApplicationDbContext, ApplicationDbContext>()", jer radim sa Repository pattern da bi se 
        IApplicationDbContext citao kao ApplicationDbContext.   
          
          Repository pattern omogucava da u Handler klasi ne definisemo logiku pristupa i writing/reading to DB, vec to u Repository klasi
        tj u nasem slucaju u ApplicationDbContext, a u Basket slucaju je to bio BasketRepository.
           
          IApplicationDbContext je definisan u Ordering.Application layeru zbog Clean architecture.

          DbContext se koristi,jer Ordering koristi SQL Server (SQL) bazu, a to je pandan IDocumentSession za Marten NoSQL bazu, 
        samo sto za razliku od IDocumentSession, ovde mogu definisati ime tabele. Kao IDocumentSession, DbContext
        ima takodje commit i rollback u SaveChangesAsync built-in metodi.
        
          Kod Basket, imao sam Repository pattern takodje kao i ovde, ali sam imao NoSQL bazi (IDocumentSessionn umesto DbContext), stoga 
        u Program.cs nisam imao AddDbContext<BasketDbContext>. Basket ima Vertical slice architecture, pa sam IBasketRepository i 
        BasketRepository stavio u isti layer (API layer), dok ovde to nije slucaj. Jer moracu ApplicationDbContext u Infrastructure layer
        a IApplicationDbContext u Application layer. ApplicationDbContext je pandan BasketRepository, dok IApplicationDbContext je pandan
        IBasketRepository. 
        
          Kod Discount, nisam imao Repository pattern, ali sam imao SQLite bazu i zato u Program.cs imao sam 
        "builder.Services.AddDbContext<DiscountDbContext> 
        */

        // Kod Discount svaka tabela imala je {get;set;}, dok ovde ce imati samo expression-bodied getter. 
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        // Mora ovakav ctor kao kod Discount sto je morao.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /* Znam iz CollegeApp i Discount, da ova metoda se sama ponudi kad krenem da je kucam. 
        Ona Seeduje initial data u orderingdb bazu i da dodam uslove za zeljene kolone
       u tabeli ako zelim. Seeduje se samo za development fazu, bas kao kod Catalog sto sam radio. */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   /* U CollegeApp imao sam modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired().HasMaxLength(100)"
             ali tako necu raditi u ovoj metodi, jer morao bih za sve 4 tabele da definisem uslove za zeljene kolone i onda
            bi telo metode bilo ogromno. Vec cu da za svaku tabelu napravim CustomerConfiguration.cs, ProductCOnfiguration.cs,
            OrderConfiguration.cs i OrderItemConfiguration.cs gde cu definisati uslove za zeljene kolone ovih tabela. */
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
