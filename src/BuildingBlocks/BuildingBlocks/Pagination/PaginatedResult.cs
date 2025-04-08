
namespace BuildingBlocks.Pagination
{
    public class PaginatedResult<TEntity>
        (int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        where TEntity : class
    {   
        /* Pravim public get polja iz ovih argumenta from primary constructor jer mi treba 
         da im pristupim. Mogo sam i bez Primary ctor, ali onda moralo je ctor da se pise.*/
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public long Count { get; } = count;
        public IEnumerable<TEntity> Data { get; } = data;
        // Polja nemaju public set, znaci da moraju se postaviti samo tokom pravljenja objekta ove klase
    }
}
