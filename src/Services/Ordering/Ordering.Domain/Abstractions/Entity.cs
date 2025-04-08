
namespace Ordering.Domain.Abstractions
{   
    // Entity mora naslediti svaka tabela u bazi zbog DDD.
    public  abstract class Entity<T> : IEntity<T>
    { // IEntity<T> => mora Entity<T>
        
        // Mora implementirati sve iz IEntity<T> i IEntity, jer IEntity<T>:IEntity
        public T Id { get; set; } // I Property se iz Entity mora implementirati u klasi 
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        // Posto sve ima set, moram da explicitno definisem ova polja prilikom pravljenja
    }
}
