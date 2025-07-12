
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Enums;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{   
    // Objasnjeno u CustomerConfiguration.cs.
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o  => o.Id); 
            builder.Property(o => o.Id).HasConversion(
                orderId => orderId.Value, // For writing in DB
                dbId => OrderId.Of(dbId)  // For reading from DB
            );

            // Relacija FK-PK za Order-Customer 
            builder.HasOne<Customer>() // 1 Order pripada 1 Customer
                   .WithMany()         // 1 Customer moze imati vise Orders
                   .HasForeignKey(o => o.CustomerId) 
                   .IsRequired();

            // Relacija FK-PK Order-OrderItem jer OrderItems polja u Order je Navigational attribute. Ovo nisam mogo ja msm napisati u OrderItemConfiguration
            builder.HasMany(o => o.OrderItems) // 1 Order moze imati vise OrderItems jer OrderItems je lista
                   .WithOne()                  // 1 OrderItem pripada samo 1 Orderu
                   .HasForeignKey(oi => oi.OrderId); 

            // TotalPrice je expression bodied property i onda mora ovako da se uradi, jer u suportnom nece EF Core da napravi tu kolonu u tabeli
            builder.Property(o => o.TotalPrice);

            // Status kolona je Enum type i zelim da vrednosti budu string,a ne 1,2,3,4 i zato mora rucno. Ista fora kao kod CustomerId u CustomerConfiguration.
            builder.Property(o => o.Status)
                   .HasConversion( 
                        s => s.ToString(), // For writing to DB to store it as a string.
                        dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus) // For reading from DB. Mora konverzija from string to enum, jer Status je OrderStatus(enum) type
                    );

            // Complex type (custom type) OrderName polje iz Order.cs se ovako pravi ako zelim da bude kolona u tabeli
            builder.ComplexProperty(
                o => o.OrderName, nameBuilder =>
                {   // Value polje iz OrderName.cs
                    nameBuilder.Property(n => n.Value)  // Value polje iz OrderName.cs postaje kolona u Orders tabeli
                               .HasColumnName(nameof(Order.OrderName)) // Dodelim ime ove kolone, ali normalno da ocu da se zove OrderName
                               .HasMaxLength(100) // Ova i linije ispod su uslovi za OrderName kolonu
                               .IsRequired();
                });

            // Complex type (custom type) ShippingAddress polje iz Order.cs se ovako pravi ako zelim da bude kolona tabele
            builder.ComplexProperty(
                o => o.ShippingAddress, addressBuilder =>
                {   // FirstName polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.FirstName) 
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za FirstName kolonu
                                  .IsRequired();
                    // LastName polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.LastName)
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za LastName kolonu
                                  .IsRequired();
                    // EmailAddress polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.EmailAddress)
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za EmailAddress kolonu
                                  .IsRequired();
                    // AddressLine polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.AddressLine)
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za AddressLine kolonu
                                  .IsRequired();
                    // Country polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.Country)
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za Country kolonu
                                  .IsRequired();
                    // State polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.State)
                                  .HasMaxLength(50) // Ovo i linija ispod su uslovi za State kolonu
                                  .IsRequired();
                    // ZipCode polje iz Address.cs postaje FirstName kolona u Orders tabeli
                    addressBuilder.Property(a => a.ZipCode)
                                  .HasMaxLength(5) // Ovo i linija ispod su uslovi za ZipCode kolonu
                                  .IsRequired();
                });

            // Isto vazi kao za ShippingAddress
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

            // Isto vazi kao za ShippingAddress
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
