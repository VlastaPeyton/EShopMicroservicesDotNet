

using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {   // Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext

        // Mora metoda zbog interface
        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        { // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            var pageIndex = query.paginationRequest.PageIndex;
            var pageSize = query.paginationRequest.PageSize;

            var totalCount = await dbContext.Orders.LongCountAsync(cancellationToken); // Built-in koja racuna br vrsta u tabeli

            var orders = await dbContext.Orders.Include(o => o.OrderItems)
                                                // Include every Order from Orders table with its OrderItems
                                                .OrderBy(o => o.OrderName.Value)
                                                /* OrderName custom type i ne moze se poredi, ali njegovo Value polje je string i moze
                                                 jer implicitno nasledimo IComparable*/
                                                .Skip(pageSize * pageIndex)
                                                .Take(pageSize)
                                                .ToListAsync(cancellationToken);

            // typeof(orders) = List<Order>, ali treba mi List<OrderDTO> zbog GetOrdersResult

            return new GetOrdersResult( new PaginatedResult<OrderDTO>(pageIndex, pageSize, totalCount, orders.ToOrderDtoList() ));
        }
    }
}
