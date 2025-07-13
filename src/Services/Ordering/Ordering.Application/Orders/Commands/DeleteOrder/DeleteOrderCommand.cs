

using System.Windows.Input;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{    // Objasnjeno u CreateOrder
    
    public record DeleteOrderCommand(Guid OrderId) : ICommand<DeleteOrderResult>;
    public record DeleteOrderResult(bool IsSuccess);
    public class DeleteOrderCommandValidator  : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator() 
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId ne sme prazan");
            // Mogo sam jos kolone da valdiitar ali necu
        }
    }
}
