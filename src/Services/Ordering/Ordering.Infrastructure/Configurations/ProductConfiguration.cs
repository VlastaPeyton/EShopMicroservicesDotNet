
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    /* Definisem uslove za Products tabelu umesto da ih, sa uslovima za ostale tabele,
    pisem u ApplicationDbContext OnModelCreating metodi.  */
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        // Ova metoda mora zbog interface. 
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Za zeljene kolone iz Product.cs (Id, Name i Price) definisemo uslove

            builder.HasKey(p => p.Id); // PK je uvek Id kolona iz Entity klase
            /* Id je tipa ProductId,a to je custom type (Value Object) koji ima Value polje. 
           SQL baza ne zna automatski kako da u kolonu stavi custom tip, pa moram definisati taj proces 
           skladistenja sa HasConversion. */
            builder.Property(p => p.Id).HasConversion(
                productId => productId.Value, // For writing to DB
                dbId => ProductId.Of(dbId) // For reading from DB
                );

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

            /* Nisam nista uradio za Price, jer nije potrebno obzirom da je to polje {get;set;} tj da ima setter 
            stoga ce autoamtski da se napravi u tabeli  */

            /* U OrderItemConfiguration.cs definisan je za tu klasu FK (ProductId polje) koje gadja PK u ovoj klasi. 
             Stoga sta god zamenio u Products tabeli odrazice se na OrderItems tabelu i obratno.*/
        }
    }
}
