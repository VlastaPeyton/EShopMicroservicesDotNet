
using System.Linq;
using Ordering.Application.DTOs;
using Ordering.Domain.Models;

namespace Ordering.Application.Extensions
{   /* Extension method for IEnumerable<Order>, jer cu u GetOrderByCustomer da koristim 
    modifikovani ProjectOrderToOrderDto (koji je def u GetOrdersByNameQueryHandler), pa da ne pisem 
    opet tu metodu samo modified. 
    
      Sada mogu izbrisati ProjectOrderToDto metodu iz GetOrdersByNameQueryHandler, ali necu, nek stoji. */
    public static class OrderExtensions
    {
        public static IEnumerable<OrderDTO> ToOrderDtoList(this IEnumerable<Order> orders)
        {   /* orders.Select(order => ...) iterira kroz svaki Order iz orders i pravi od njega OrderDTO */
            return orders.Select(order => new OrderDTO(
                Id: order.Id.Value, 
                CustomerId: order.CustomerId.Value,
                OrderName: order.OrderName.Value,
                ShippingAddress: new AddressDTO(order.ShippingAddress.FirstName,
                                                order.ShippingAddress.LastName,
                                                order.ShippingAddress.EmailAddress,
                                                order.ShippingAddress.AddressLine,
                                                order.ShippingAddress.Country,
                                                order.ShippingAddress.State,
                                                order.ShippingAddress.ZipCode),
                // Mora ovim redosledom jer AddressDTO tako zahteva
                BillingAddress: new AddressDTO(order.BillingAddress.FirstName,
                                                order.BillingAddress.LastName,
                                                order.BillingAddress.EmailAddress,
                                                order.BillingAddress.AddressLine,
                                                order.BillingAddress.Country,
                                                order.BillingAddress.State,
                                                order.BillingAddress.ZipCode),
                // Mora ovim redosledom jer AddressDTO tako zahteva
                Payment: new PaymentDTO(order.Payment.CardName,
                                        order.Payment.CardNumber,
                                        order.Payment.Expiration,
                                        order.Payment.CCV,
                                        order.Payment.PaymentMethod),
                // Mora ovim redosledom jer PaymentDTO tako zahteva
                Status: order.Status,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDTO(oi.OrderId.Value, oi.ProductId.Value, oi.Quantity, oi.Price)).ToList()
                ));
            // Sa leve strane : sam pisao imena argumenta iz OrderDTO i moram bas kao u toj klasi da ih napisem ako ih vec pisem
        }

        public static OrderDTO ToOrderDto(this Order order)
        {   
            //Zbog zaglavlja, ToOrderDto nema argumenta

            return DtoFromOrder(order);
        }

        private static OrderDTO DtoFromOrder(Order order)
        {
            return new OrderDTO(
                Id: order.Id.Value,
                CustomerId: order.CustomerId.Value,
                OrderName: order.OrderName.Value,
                ShippingAddress: new AddressDTO(order.ShippingAddress.FirstName,
                                                order.ShippingAddress.LastName,
                                                order.ShippingAddress.EmailAddress,
                                                order.ShippingAddress.AddressLine,
                                                order.ShippingAddress.Country,
                                                order.ShippingAddress.State,
                                                order.ShippingAddress.ZipCode),
                // Mora ovim redosledom jer AddressDTO tako zahteva
                BillingAddress: new AddressDTO(order.BillingAddress.FirstName,
                                                order.BillingAddress.LastName,
                                                order.BillingAddress.EmailAddress,
                                                order.BillingAddress.AddressLine,
                                                order.BillingAddress.Country,
                                                order.BillingAddress.State,
                                                order.BillingAddress.ZipCode),
                // Mora ovim redosledom jer AddressDTO tako zahteva
                Payment: new PaymentDTO(order.Payment.CardName,
                                        order.Payment.CardNumber,
                                        order.Payment.Expiration,
                                        order.Payment.CCV,
                                        order.Payment.PaymentMethod),
                 // Mora ovim redosledom jer PaymentDTO tako zahteva)
                 Status: order.Status,
                 OrderItems: order.OrderItems.Select(oi => new OrderItemDTO(oi.OrderId.Value, oi.ProductId.Value, oi.Quantity, oi.Price)).ToList()
                );
        }
    }
}
