
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Extensions;
using Ordering.Domain.Models;

namespace Ordering.Application.Orders.Querys.GetOrdersByName
{   // Objasnjeno u GetOrders
    public class GetOrdersByNameQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {   
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {   
            // Get orders by name 
            var orders = await dbContext.Orders.Include(o => o.OrderItems) // Include jer OrderItems je navigational attribute i ovo je eager loading
                                               .AsNoTracking() // When reading from DB without updating entities ne treba change tracker jer samo usporava i zauima memoriju
                                               .Where(o => o.OrderName.Value.Contains(query.Name)) // Veci spektar resenja sa contains nego sa == 
                                               .OrderBy(o => o.OrderName.Value) 
                                               // OrderName.cs ima polje Value, koje je string i implicitno nasledi IComparable pa moze da se poredi
                                               .ToListAsync(cancellationToken);

            
            return new GetOrdersByNameResult(orders.ToOrderDtoList());
        }
        
    }
}
