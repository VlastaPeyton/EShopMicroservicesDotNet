

using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrders
{   
    public class GetOrdersQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {   // Neam Repository, pa zato IApplicationDbContext koristim i logiku pisem ovde. U Infrastructure layer IApplicationDbContext registrovan kao ApplicaitonDbContext.

        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        { // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            var pageIndex = query.paginationRequest.PageIndex;
            var pageSize = query.paginationRequest.PageSize;

            var totalCount = await dbContext.Orders.LongCountAsync(cancellationToken); // Built-in koja racuna br vrsta u tabeli

            var orders = await dbContext.Orders.Include(o => o.OrderItems) // Mora Include jer OrderItems je navigational attribute i eager loading
                                                .AsNoTracking() // Change tracker mi ne treba, jer ne modifikujem bazu,a samo zauima memoriju i usporava
                                                .OrderBy(o => o.OrderName.Value)
                                                // OrderName custom type i ne moze se poredi, ali njegovo Value polje je string i mozejer implicitno nasledimo IComparable*/
                                                .Skip(pageSize * pageIndex)
                                                .Take(pageSize)
                                                .ToListAsync(cancellationToken);

            return new GetOrdersResult( new PaginatedResult<OrderDTO>(pageIndex, pageSize, totalCount, orders.ToOrderDtoList() ));
        }
    }
}
