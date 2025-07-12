

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.Domain.Abstractions;

namespace Ordering.Infrastructure.Data.Interceptors
{
    /* Kada u Application layer Command/QueryHandler (posto neam Repository) pristupam nekoj vrsti iz tabele kroz LINQ i ako ne koristim AsNoTracking (sto je default opcija), Change Tracker in EF Core will track tu vrstu tj njene promene i nakon SaveChangesAsync ce da ih upise u bazu.
    Na osnovu tipa promene, Change Tracker kod sebe zabelezi za tu vrstu odredjene promene. State moze biti:
               1) Added - entity object(vrsta tabelee) added to DbContext and will be inserted to DB when SaveChangeAsync is called
               2) Modified - entity object(vrsta tabelee) modified and changes will be updated to DB when SaveChangesAsync is called
               3) Deleted - entity object(vrsta tabelee) marked for deletetion and will be deleted from DB when SaveChangesAsync is called 
               4) Unchanged - not modified tabela 
               5) Detached - entity object(vrsta tabele) not tracked by DbContext
     Ako koristim AsNoTracking, EF Core Change Tracker wont track, i nakon SaveChangesAsync nece se videti promena u bazi. 

     Interceptor sluzi da iz Change Tracker uhvati tip promene i da na osnovu toga stavi vrednost u Autid kolone (CreatedAt, CreatedBy, ModifiedOn, ModifiedBy) u svaku Models klasu jer je svaka nasledila Entity (tj IEntity) */
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {   // u DepdencyInjection.cs (Program.cs) mora da se registruje  da AuditableEntityInterceptor  se odnosi na ISaveChangesInterceptor

        // Kucam public override i samo nam ponudi gomilu override metoda i mi biramo ove 2
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            // Update tabelu before saving changes
            UpdateEntities(eventData.Context); // eventData.Context = ApplicationDbContext
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
        {   // Mora ValueTask jer override to zahteva

            // Update tabelu before saving changes 
            UpdateEntities(eventData.Context); // eventData.Context = ApplicationDbContext
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            // ApplicationDbContext nasledio DbContext pa uvek koristimo roditelja
            if (context is null)
                return;

            /* EntityEntry type (tj entry) je svaka vrsta koju change tracker tracks, a nas zanimaju samo njene Audit kolone i zato mora Entries<IEntity> jer ChangeTracker.Entires<IEntity>
            nalazi klase koje nasledile IEntity, a to su tabele. 
              */
            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added) // If new row is added in table Change Tracker assings it Added state
                {
                    entry.Entity.CreatedBy = "Vlasta modifikovao Order"; 
                    entry.Entity.CreatedAt = DateTime.UtcNow; 
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HaChangedOwnedEntities()) // HasChangedOwnedEntities se odnosi na Order Update metodu jer ona menja Owned entites
                {
                    entry.Entity.LastModifiedBy = "Vlasta zameni OrderItems listu"; 
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;  
                }
            }
        }
    }

    /* Pravim extension method za EntityEntry jer entry in UpdateEntites je EntityEntry type (vrsta tabele je ovog tipa). */
    public static class Extensions
    {
        public static bool HaChangedOwnedEntities(this EntityEntry entry) =>
            // entry.References vraca sve Owned entties,a to moze samo za Order, jer on to sadrzi. Owned entity nije OrderItem polje, jer je to navigaitonal attribute. To je Shipping/BillingAddress, OrderName, Payment and Status. Jer se Update metodom tako order modifikuje.
            entry.References.Any(r => // Proverava da li navigacioni atribut zadovoljava sledece uslove
                r.TargetEntry != null && 
                r.TargetEntry.Metadata.IsOwned() &&  // Da li je entity owned. Ovo nije OrderItems in Order, vec OrderName, Shipping/BillingAddress,Payment and Status
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified)
            ); 
    }
    
}
