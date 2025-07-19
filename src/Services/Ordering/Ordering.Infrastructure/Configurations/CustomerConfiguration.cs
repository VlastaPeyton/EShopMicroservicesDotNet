
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Configurations
{
    /* Umesto da uslove zeljenih kolona tabele, Seeding, definicja PK,FK, PK-FK relacije, Indexing definisem u OnModelCreating, to cu odraditi ovde, kako 
     zbog 4 tabele, OnModelCreating ne bi bio ogroman. */
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {   
        // Ova metoda mora zbog interface. 
        public void Configure(EntityTypeBuilder<Customer> builder)
        {   
            /* Customer tabela ima Id polje koje nije Guid ili int tipa, vec custom type (strongly-typed Id tj Value Object) i moram rucno definisati PK i koji tip ce biti u Id koloni.
            jer SQL baza ne zna autoamtski da u Id kolonu stavi custom type. */
            builder.HasKey(c => c.Id); 
            builder.Property(c => c.Id).HasConversion(
                customerId => customerId.Value, // For writing in DB. Value je Guid i zato ce Id kolona biti Guid.
                dbId => CustomerId.Of(dbId)     // For reding from DB. Id field of Customer je CustomerId tipa, a u bazi je Id kolona Guid type, ali pri citanju moram vratiti u CustomerId type, jer Of metoda tome sluzi.
            ); // Zbog nemanja ValueGeneratedOnAdd ovde, EF Core nece automatski generisati Id za Customer tabelu

            // Primitive type property of Customer.cs postaju kolone automatski, ali nekad moram nekim kolonama uslove da zadam
            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(255);

            // Ako Customer.cs ima expression-bodied property i zelim da to polje bude kolona, moram to explicitno napisati ovde. Videti u OrderConfiguration.
            
            // Ako Customer.cs ima custom type(Value Object) property (kao Order.cs sto ima ShippingAddress), to se mora rucno definise ako zelim da njegova polja budu kolone u Orders tabeli. Videti u OrderConfiguration.

            /* DB Indexing:
                  Svaka Id (PK) je autoamtski Index, pa u LINQ query by Id je brz veoma tj najbrzi moguci jer najbrze pretrazuje by integer. 
                  Onaj Endpoint koji se, zbog FE, veoma cesto koristi, a pritom njegov LINQ queries DB by column which is not PK, mogu setovati tu kolonu kao Index da bi bazu na osnovu toga brze pretrazio. 
                Ovo ima smisla uvek, ali narocito ako se cesto koristi taj Endpoint. 
                  Kada je neka kolona postavljena kao Index, napravi se data structure (lookup table) koja vrsti binary search O(logn) trajanja, dok ako pretrazujem bazu preko te kolone to je O(n) jer prolazi kroz sve vrste.
                  Indexed column se koristi u LINQ samo kad imamo == ili StartsWith, dok za Contains or EndsWith Indexing string column ne pomaze.
             */
            // Indexing of Customers table, jer cesto je koristim i pretrazujem i ocu da ubrzam to pomocu indexing
            builder.HasIndex(c => c.Email).IsUnique();

            // U OrderConfiguration.cs definisan je FK (CustomerId polje of Order.cs) koje gadja PK u ovoj klasi. Stoga sta god zamenio u Orders tabeli odrazice se na Customers tabelu i obratno.*/
        }
    }
}
