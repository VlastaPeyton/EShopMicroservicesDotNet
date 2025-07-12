
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    // Objasnjeno sve u CustomerConfiguration.cs 
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {   

            builder.HasKey(oi => oi.Id); 
            builder.Property(oi => oi.Id).HasConversion(
                orderItemId => orderItemId.Value, // For writing to DB
                dbId => OrderItemId.Of(dbId)      // For reading from DB
            );

            // OrderId ima FK1=ProductId i FK2=OrderId. Ovde cu definisati samo FK1-PK za OrderItem-Product. Dok FK2-PK za OrderItem-Order definisacu u OrderConfiguration, jer OrderItems polje of Order je navigational attribute i ta relacija mora tamo biti definisana.
            builder.HasOne<Product>() // 1 OrderItem moze biti(imati) samo 1 Product
                   .WithMany()  // 1 Product moze biti odabran kao vise OrderItems ako vise komada nam treba
                   .HasForeignKey(oi => oi.ProductId); 

            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.Price).IsRequired();

            // U OrderConfiguration definisan veza za Id polje iz Order.cs (PK za Order.cs) da gadja FK2 u OrderItem (OrderId polje), stoga ako nesto modifikujem u Orders tabeli bice OrderItems tabela afektovana i obratno
        }
    }
}
