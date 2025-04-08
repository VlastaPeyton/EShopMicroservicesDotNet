
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Enums;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{   /* Definisem uslove za Orders tabelu umesto da ih, sa uslovima za ostale tabele,
    pisem u ApplicationDbContext OnModelCreating metodi.  */
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        // Ova metoda mora zbog interface.

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            /* Za zeljene kolone iz Order.cs(Id, OrderItems, CustomerId, OrderName, ShippingAddress,
             BillingAddress, Payment, TotalPrice or  Status) definisem uslove. */

            builder.HasKey(o  => o.Id); // PK je uvek Id kolona iz Entity klase
            /* Id je tipa OrderId,a to je custom type (Value Object) koji ima Value polje. 
          SQL baza ne zna automatski kako da u kolonu stavi custom tip, pa moram definisati taj proces 
          skladistenja sa HasConversion. */
            builder.Property(o => o.Id).HasConversion(
                orderId => orderId.Value, // For writing in DB
                dbId => OrderId.Of(dbId)  // For reading from DB
                );

            // Relacija Order-Customer 
            builder.HasOne<Customer>() // 1 Order pripada 1 Customer
                   .WithMany()         // 1 Customer moze imati vise Orders
                   .HasForeignKey(o => o.CustomerId) /*  Customer polje iz Order.cs je 
                        FK za Orders tabelu koji gadaj PK (Id polje) u Customers tabeli, stoga sta god izmenio u Orders tabeli 
                        odrazice se na Customers tabelu i obratno. Zato, u Postman prilikom "POST CreateOrder" 
                        moram CustomerId uneti neki od postojecih iz CustomerId. */
                   .IsRequired();

            // Relacija Order-OrderItem
            builder.HasMany(o => o.OrderItems) // 1 Order moze imati vise OrderItems
                   .WithOne()                  // 1 OrderItem pripada samo 1 Orderu
                   .HasForeignKey(oi => oi.OrderId); /* OrderId polje iz OrderItem.cs je FK koji gadja PK (Id polje) iz Order.cs 
                    OrderId, stoga ako nesto modifikujem u Orders tabeli afektovace i OrderItems tabelu i obratno.
                    Zat, u Postman "PUT UpdateOrder" moram uneti postojeci OrderId iz Orders tabele koja ima Seedovano 2 Ordera kroz
                    InitialData.cs (ali imacu jos jedan Order dodat nakon "POST CreateOrder" u Postman. 
                   */

            /* TotalPrice je expression bodied getter (no setter) i onda mora ovako da se navede iako nema uslov za njega, jer u suportnom
             nece EF da napravi tu kolonu u tabeli. */
            builder.Property(o => o.TotalPrice);

            // Enum polje iz Order.cs 
            builder.Property(o => o.Status)
                   .HasDefaultValue(OrderStatus.Draft)
                   .HasConversion(
                        s => s.ToString(), // For writing to DB
                        dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus) // For reading from DB
                    );

            // Complex type (custom type) polje iz Order.cs se ovako mapira da bude kolona tabele
            builder.ComplexProperty(
                o => o.OrderName, nameBuilder =>
                {   // Value polje iz OrderName.cs
                    nameBuilder.Property(n => n.Value) 
                               .HasColumnName(nameof(Order.OrderName))
                               .HasMaxLength(100)
                               .IsRequired();
                });

            // Complex type (custom type) polje iz Order.cs se ovako mapira da bude kolona tabele
            builder.ComplexProperty(
                o => o.ShippingAddress, addressBuilder =>
                {   // FirstName polje iz Address.cs
                    addressBuilder.Property(a => a.FirstName) 
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // LastName polje iz Address.cs 
                    addressBuilder.Property(a => a.LastName)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // EmailAddress polje iz Address.cs 
                    addressBuilder.Property(a => a.EmailAddress)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // AddressLine polje iz Address.cs 
                    addressBuilder.Property(a => a.AddressLine)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // Country polje iz Address.cs 
                    addressBuilder.Property(a => a.Country)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // State polje iz Address.cs 
                    addressBuilder.Property(a => a.State)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // ZipCode polje iz Address.cs 
                    addressBuilder.Property(a => a.ZipCode)
                                  .HasMaxLength(5)
                                  .IsRequired();
                });

            // Complex type (custom type) polje iz Order.cs se ovako mapira da bude kolona tabele
            builder.ComplexProperty(
                o => o.BillingAddress, addressBuilder =>
                {   // FirstName polje iz Address.cs
                    addressBuilder.Property(a => a.FirstName)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // LastName polje iz Address.cs 
                    addressBuilder.Property(a => a.LastName)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // EmailAddress polje iz Address.cs 
                    addressBuilder.Property(a => a.EmailAddress)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // AddressLine polje iz Address.cs 
                    addressBuilder.Property(a => a.AddressLine)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // Country polje iz Address.cs 
                    addressBuilder.Property(a => a.Country)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // State polje iz Address.cs 
                    addressBuilder.Property(a => a.State)
                                  .HasMaxLength(50)
                                  .IsRequired();
                    // ZipCode polje iz Address.cs 
                    addressBuilder.Property(a => a.ZipCode)
                                  .HasMaxLength(5)
                                  .IsRequired();
                });
            // Complex type (custom type) polje iz Order.cs se ovako mapira da bude kolona tabele
            builder.ComplexProperty(
                o => o.Payment, paymentBuilder =>
                {   // CardName polje iz Payment.cs 
                    paymentBuilder.Property(p => p.CardName)
                                  .HasMaxLength(50);
                    // CardNumber polje iz Payment.cs 
                    paymentBuilder.Property(p => p.CardNumber)
                                  .HasMaxLength(25)
                                  .IsRequired();
                    // Expiration polje iz Payment.cs 
                    paymentBuilder.Property(p => p.Expiration)
                                  .HasMaxLength(10);
                    // CCV polje iz Payment.cs 
                    paymentBuilder.Property(p => p.CCV)
                                  .HasMaxLength(3)
                                  .IsRequired();
                    // PaymentMethod polje iz Payment.cs 
                    paymentBuilder.Property(p => p.PaymentMethod);
                });
        }
    }
}
