
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.DTOs;
using Ordering.Application.Extensions;
using Ordering.Domain.Models;

namespace Ordering.Application.Orders.Querys.GetOrdersByName
{
    public class GetOrdersByNameQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {   // Kao kod Basket imamo Repository pattern,  samo sto tamo CommandHandler (IBasketRepository repository),a ovde DbContext 
        
        // Mora metoda zbog interface
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {   // Trebalo bi zbog Repository pattern, da ovu logiku smestimo u neku metodu iz IApplicaitonDbContext (definisau u ApplicationDbContext)

            // Get orders by name 
            var orders = await dbContext.Orders.Include(o => o.OrderItems) // Include every Order from Orders table with its OrderItems in query
                                               .AsNoTracking() // When reading from DB without updating entities
                                               .Where(o => o.OrderName.Value.Contains(query.Name)) // Veci spektar resenja sa contains nego sa == 
                                               .OrderBy(o => o.OrderName.Value) 
                                               // OrderName.cs ima polje Value, koje je string i implicitno nasledi IComparable pa moze da se poredi
                                               .ToListAsync(cancellationToken);

            /* typeof(orders) = List<Order>, ali nama treba List<OrderDTO> zbog GetOrdersByNameResult, pa moram da napravim metodu koja ce to da konvertuje
             jer i kod Query razdvajan Application od Domain layera */
            //var orderDTOs = ProjectToOrderDto(orders); - Ovo sam koristio pre nego sto sam napraivo ToOrderDtoList extension method 
            //return new GetOrdersByNameResult(orderDTOs);-  Ovo sam koristio pre nego sto sam napraivo ToOrderDtoList extension method 
            
            return new GetOrdersByNameResult(orders.ToOrderDtoList());
        }
        // Kako sam napravio ToOrderDtoList extension method, ova metoda vise ne treba, ali nek stoji ovde
        private List<OrderDTO> ProjectToOrderDto(List<Order> orders)
        {
            List<OrderDTO> result = new(); // isto kao new List<OrderDTO>()
            
            foreach (var order in orders)
            {
                var shippingAddress = new AddressDTO(order.ShippingAddress.FirstName,
                                                     order.ShippingAddress.LastName,
                                                     order.ShippingAddress.EmailAddress,
                                                     order.ShippingAddress.AddressLine,
                                                     order.ShippingAddress.Country,
                                                     order.ShippingAddress.State,
                                                     order.ShippingAddress.ZipCode);
                // Mora ovim redom jer tako zahteva AddressDTO 

                var billingAddress = new AddressDTO( order.BillingAddress.FirstName,
                                                     order.BillingAddress.LastName,
                                                     order.BillingAddress.EmailAddress,
                                                     order.BillingAddress.AddressLine,
                                                     order.BillingAddress.Country,
                                                     order.BillingAddress.State,
                                                     order.BillingAddress.ZipCode);
                // Mora ovim redom jer tako zahteva AddressDTO 

                var payment = new PaymentDTO(order.Payment.CardName,
                                             order.Payment.CardNumber,
                                             order.Payment.Expiration,
                                             order.Payment.CCV,
                                             order.Payment.PaymentMethod);
                // Mora ovim redom jer tako zahteva PaymentDTO

                var orderDto = new OrderDTO(Id: order.Id.Value,
                                            CustomerId: order.CustomerId.Value,
                                            OrderName: order.OrderName.Value,
                                            ShippingAddress: shippingAddress,
                                            BillingAddress: billingAddress,
                                            Payment: payment,
                                            Status: order.Status,
                                            OrderItems: order.OrderItems.Select(oi => new OrderItemDTO(oi.OrderId.Value, oi.ProductId.Value, oi.Quantity, oi.Price)).ToList()
                                            );
                // Levo od : su imena polja iz OrderDTO i moramo ih bas takva napisati ako vec ih pisemo

                result.Add(orderDto); // Add je built-in za List
            }

            return result;
        }
    }
}
