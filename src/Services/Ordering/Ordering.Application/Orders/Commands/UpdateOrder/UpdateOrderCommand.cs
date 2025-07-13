
using System.Windows.Input;
using BuildingBlocks.CQRS;
using FluentValidation;
using Ordering.Application.DTOs;
using Ordering.Domain.Models;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{   // Objasnjeno u CreateOrder 
    public record UpdateOrderCommand(OrderDTO Order) : ICommand<UpdateOrderResult>;
    public record UpdateOrderResult(bool IsSuccess);

    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Order.Id).NotEmpty().WithMessage("Id requeired");
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName requeired");
            RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId required");
            // Mogo sam jos kolona validirati, ali necu
        }
    }
}
