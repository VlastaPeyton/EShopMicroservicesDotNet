
namespace Ordering.Domain.Enums
{   // U OnModelCreating tj OrderConfiguration.cs ako rucno ne namestim, u koloni Status Orders tabele stajace 1,2,3,4 umesto stringova
    public enum OrderStatus
    {
        Draft = 1,
        Pending = 2,
        Completed = 3,
        Canceled = 4
    }
}
