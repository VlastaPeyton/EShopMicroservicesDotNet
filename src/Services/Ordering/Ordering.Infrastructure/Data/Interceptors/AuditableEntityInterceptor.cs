

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.Domain.Abstractions;

namespace Ordering.Infrastructure.Data.Interceptors
{   
    // Interceptor da popuni Audit kolone (iz IEntity) za svaku tabelu. 
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {   /* u DepdencyInjection.cs (Program.cs) mora da se registruje  da AuditableEntityInterceptor 
         se odnosi na ISaveChangesInterceptor> */

        // Kucam public override i samo nam ponudi gomilu metoda i mi biramo ove 2
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            // Update tabelu before saving changes
            UpdateEntities(eventData.Context); // eventData.Context = ApplicationDbContext
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            // Update tabelu before saving changes 
            UpdateEntities(eventData.Context); // eventData.Context = ApplicationDbContext
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            // ApplicationDbContext nasledio DbContext pa uvek koristimo roditelja
            if (context is null)
                return;

            /* U IEntity.cs sve objasnjeno detaljno za Audit kolone 
             EF, pomocu ChangeTracker, prati promene u tabeli i na osnovu toga stavlja vrednosti u Audit kolone. 
             EntityEntry je Change Tracking Information za svaku vrstu iz tabele koja je nasleidla IEntity. 
             Owned Entity postoji samo u Order klasi i to je OrderItem (jer Payment/Address nisu nasledile Entity.cs iako su
            kao i OrderItem polja od Order). 
               
              Entry(EntityEntry) je svaka vrsta tabele, a nas zanimaju samo Audit kolone i zato Entries<IEntity> jer ChangeTracker.Entires<IEntity>
            nalazi klase koje nasledile IEntity tj nase tabele nalazi. */
            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added) // If new row is added in table
                {
                    entry.Entity.CreatedBy = "mehmet"; // Audit kolona iz IEntity
                    entry.Entity.CreatedAt = DateTime.UtcNow; // Audit kolona iz IEntity
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HaChangedOwnedEntities())
                {
                    entry.Entity.LastModifiedBy = "mehmet"; // Audit kolona iz IEntity
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;  // Audit kolona iz IEntity
                }
            }
        }
    }
    /* Pravim extension method za EntityEntry je Change Tracking Information za svaku vrstu iz tabele koja je nasleidla IEntity. */
    public static class Extensions
    {
        public static bool HaChangedOwnedEntities(this EntityEntry entry) =>
            // entry.References vraca sve navigacione reference ovog entiteta
            entry.References.Any(r => // Prolazim kroz sve reference glavnog entiteta
                r.TargetEntry != null && // Provera da li referenca ukazuje na entitet
                r.TargetEntry.Metadata.IsOwned() &&  // Da li je entity owned (OrderItems polje u Order.cs)
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
                /* Da li je owned entity added or modified tj da l sam dodao OrderItem (AddOrderItem metoda u Order.cs)
                    ili sam izbacio OrderItem (RemoveOrderItem metoda u Order.cs) */
    }
    
}
