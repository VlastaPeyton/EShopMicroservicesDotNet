
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    /* Definisem uslove za Customers tabelu umesto da ih, sa uslovima za ostale tabele,
     pisem u ApplicationDbContext OnModelCreating metodi.  */
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {   
        // Ova metoda mora zbog interface. 
        public void Configure(EntityTypeBuilder<Customer> builder)
        {   
            /* Za zeljene kolone iz Customer.cs (Id, Name i Email) definisemo uslove. */

            builder.HasKey(c => c.Id);  // PK je uvek Id kolona iz Entity klase
            /* Id je tipa CustomerId,a to je custom type (Value Object) koji ima Value polje. 
            SQL baza ne zna automatski kako da u kolonu stavi custom tip, pa moram definisati taj proces 
            skladistenja sa HasConversion. */
            builder.Property(c => c.Id).HasConversion(
                customerId => customerId.Value, // For writing in DB
                dbId => CustomerId.Of(dbId));  // For reding from DB

            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(255);
            // Index vrste nece biti 1,2,3... vec Email i zato mora unique 
            builder.HasIndex(c => c.Email).IsUnique();

            /* U OrderConfiguration.cs definisan je za tu klasu FK (CustomerId polje) koje gadja PK u ovoj klasi. 
             Stoga sta god zamenio u Orders tabeli odrazice se na Customers tabelu i obratno.*/
        }
    }
}
