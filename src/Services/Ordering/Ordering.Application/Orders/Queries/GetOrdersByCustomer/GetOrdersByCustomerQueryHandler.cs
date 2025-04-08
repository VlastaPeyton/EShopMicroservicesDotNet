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
{
    public class GetOrdersByCustomerQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {// Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext

        // Mora metoda zbog interface 
        public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        { // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)
            
            var orders = await dbContext.Orders.Include(o => o.OrderItems)
                                               // Include every Order from Orders table with its OrderItems 
                                               .AsNoTracking() // When reading from DB withoud updating entites
                                               .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
                                               /* Za zeljenog customera biramo sve njegove orders (ako ih ima vise jer sam u OrderConfiguration 
                                               namestio da 1 Customer moze imati vise Ordera */
                                               .OrderBy(o => o.OrderName.Value)
                                               /* OrderName je custom tipa i ne moze se poredi, ali njegovo Value polje je string i ono moze jer implicitno
                                               nasledilo IComparable */
                                               .ToListAsync(cancellationToken);

            // typeof(orders) = List<Order> ali nama treba List<OrderDTO> zbog GetOrdersByCustomerResult i koristim napravljen extension method 
            return new GetOrdersByCustomerResult(orders.ToOrderDtoList());
        }
    }
}
