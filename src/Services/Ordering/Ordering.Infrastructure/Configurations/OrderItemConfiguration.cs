
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    /* Definisem uslove za OrderItems tabelu umesto da ih, sa uslovima za ostale tabele,
    pisem u ApplicationDbContext OnModelCreating metodi.  */
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        // Ova metoda mora zbog interface.
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {   
            // Za zeljene kolone iz OrderItem.cs (Id, OrderId, ProductId, Quantity or Price) definisemo uslove. 

            builder.HasKey(oi => oi.Id); // PK je uvek Id kolona iz Entity klase
            /* Id je tipa OrderItemID,a to je custom type (Value Object) koji ima Value polje. 
          SQL baza ne zna automatski kako da u kolonu stavi custom tip, pa moram definisati taj proces 
          skladistenja sa HasConversion. */
            builder.Property(oi => oi.Id).HasConversion(
                orderItemId => orderItemId.Value, // For writing to DB
                dbId => OrderItemId.Of(dbId)      // For reading from DB
                );

            // Relacija OrderItem-Product
            builder.HasOne<Product>() // 1 OrderItem moze biti samo 1 Product
                   .WithMany()  // 1 Product moze biti odabran kao vise OrderItems ako vise komada nam treba
                   .HasForeignKey(oi => oi.ProductId); /*  ProductId polje iz OrderItem.cs je 
                        FK za OrderItem tabelu koji gadaj PK (Id polje) u Products tabeli, stoga sta god izmenio u OrderItems tabeli 
                        odrazice se na Products tabelu i obratno.*/

            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.Price).IsRequired();

            /* U OrderConfiguration definisan veza za  Id polje iz Order.cs (PK za Order.cs) da gadja FK2 u OrderItem (OrderId polje) 
             stoga ako nesto modifikujem u Orders tabeli bice OrderItems tabela afektovana i obratno.*/
        }
    }
}
