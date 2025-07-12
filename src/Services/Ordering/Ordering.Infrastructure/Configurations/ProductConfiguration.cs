
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    // Objasnjeno sve u CustomerConfiguration.cs 
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasConversion(
                productId => productId.Value, // For writing to DB
                dbId => ProductId.Of(dbId) // For reading from DB
                );

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

            // U OrderItemConfiguration.cs definisan je  FK (ProductId polje u OrderItem.cs) koje gadja PK u ovoj klasi. Stoga sta god zamenio u Products tabeli odrazice se na OrderItems tabelu i obratno.
        }
    }
}
