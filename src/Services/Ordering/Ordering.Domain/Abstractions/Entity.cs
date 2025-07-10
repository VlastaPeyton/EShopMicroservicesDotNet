
namespace Ordering.Domain.Abstractions
{   
    // Entity mora naslediti svaka Models klasa tj tabela u bazi zbog DDD.
    public  abstract class Entity<T> : IEntity<T>
    {   // Zelim da osiguram da ne moze objekat biti tipa ove klase i zato je abstract.
        
        // Mora implementirati sve iz IEntity<T> i IEntity, jer IEntity<T>:IEntity
        public T Id { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
