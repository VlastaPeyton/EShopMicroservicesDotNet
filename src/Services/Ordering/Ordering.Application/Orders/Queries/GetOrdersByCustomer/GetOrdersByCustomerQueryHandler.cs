using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Extensions;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;
using Ordering.Application.Orders.Querys.GetOrdersByName;
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Application.Orders.Querys.GetOrdersByCustomer
{   // Objasnio u GetOrders
    public class GetOrdersByCustomerQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {
        public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        { 
            var orders = await dbContext.Orders.Include(o => o.OrderItems) // Mora include jer OrderItems je navigational attribute i eager loading
                                               .AsNoTracking() // When reading from DB withoud updating entity jer change tracker mi ne treba kad citam iz baze obzirom da usporava poso i memory zauzima
                                               .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
                                               /* Za zeljenog customera biramo sve njegove orders (ako ih ima vise jer sam u OrderConfiguration 
                                               namestio da 1 Customer moze imati vise Ordera */
                                               .OrderBy(o => o.OrderName.Value) // OrderName je custom tipa i ne moze se poredi, ali njegovo Value polje je string i ono moze jer implicitnonasledilo IComparable 
                                               .ToListAsync(cancellationToken);

            return new GetOrdersByCustomerResult(orders.ToOrderDtoList());
        }
    }
}
