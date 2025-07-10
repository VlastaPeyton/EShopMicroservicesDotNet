

namespace Ordering.Domain.Abstractions
{   
    /*Pravim IEntity, jer je dobra praksa da sve ide preko interface, zbog DDD, jer ako sve stavim u Entity.cs, onda kad menjam, menjam Entity sto je cimanje, dok ovako interface osigurava. Ista fora kao kod IRepository i Repository zbog DI.
      Entity<T> ce implementirati IEntity<T>, jer Entity ce naslediti svaka klasa koja ce biti tabela u bazi. 
      Mogo sam da sve iz IEntity stavim u IEntity<T>, ali je ovo dobra praksa, bas kao u BuildingBLocks za ICommand sto sam uradio.  */
    public interface IEntity
    {
        /* Ovo su audit kolone u tabeli koje prate kad se neka vrsta promenila/dodala u tabelu.
           Change Tracker in EF Core assings corresponding state for each entity (row in table) it tracks in DbContext. States are:
               1) Added - entity object(vrsta tabelee) added to DbContext and will be inserted to DB when SaveChangeAsync is called
               2) Modified - entity object(vrsta tabelee) modified and changes will be updated to DB when SaveChangesAsync is called
               3) Deleted - entity object(vrsta tabelee) marked for deletetion and will be deleted from DB when SaveChangesAsync is called 
               4) Unchanged - not modified tabela 
               5) Detached - entity object(vrsta tabele) not tracked by DbContext 
         
         EF Core, pomocu Change Tracker, prati sve promene u tabelama i na osnovu toga stavlja vrednosti u ove audit kolone ispod definisane.

         EntityEntry je change tracking information za svaku vrstu tabele. 

         Videcu u Infrastructure layeru kod Interceptor ovo sta je tacno. */
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }

    }
    
    public interface IEntity<T> : IEntity
    {   /* Posto je IEntity<T> interface (a ne klasa), on ne mora implementirati sve iz IEntity, ali klasa koja implementira IEntity<T>, morace implementirati sve iz IEntity i IEntity<T>. 
        
          T jer type of Id je klasa(Value Object ili Guid). */
        public T Id { get; set; }
    }
}
