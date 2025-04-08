

namespace Ordering.Domain.Abstractions
{   
    /*Pravim IEntity, jer je dobra praksa da sve ide preko interface. 
    Entity<T> ce implementirati IEntity<T>, jer Entity ce naslediti svaka klasa koja ce biti
     tabela u bazi. 
      Mogo sam da sve iz IEntity stavim u IEntity<T>, ali je ovo dobra praksa, bas kao u BuildingBLocks
    za ICommand npr sto smo radili. 
       Entity je tabela u bazi.
       Ovo se radi zbog DDD. */
    public interface IEntity
    {   
        /* Ovo su audit kolone u tabeli koje prate kad se sta promenilo/napravilo
         tj prati se stanje tabele koje moze biti:
           1) Added - entity added to DbContext and will be inserted to DB
           2) Modified - entity modified and changes will be upldated to DB when SaveChangesAsync is called
           3) Deleted - entity marked for deletetion and will be deleted from DB when SaveChangesAsync is called 
           4) Unchanged - not modified tabela 
           5) Detached - entity not tracked by DbContext 
         
         EF pomocu ChangeTracker prati sve promene u tabelama i na osnovu toga stavlja vrednosti u audit kolone
         EntityEntry je change tracking information za svaku vrstu tabele. 
         Videcu u Infrastructure layeru kod Interceptor ovo kasnije. */
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }

    }
    
    public interface IEntity<T> : IEntity
    {   /* Posto je IEntity<T> interface (a ne klasa), on ne mora implementirati sve
         iz IEntity, ali klasa koja implementira IEntity<T>, morace implementirati sve 
        iz IEntity i IEntity<T>. 
        
          T jer type of Id je klasa(Value Object ili Guid). */
        public T Id { get; set; }
    }
}
