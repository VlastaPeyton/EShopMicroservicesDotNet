
using BuildingBlocks.CQRS;
using FluentValidation;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Commands.CreateOrder
{   // Objasnjeno u Catalog i Basket
    public record CreateOrderCommand(OrderDTO Order) : ICommand<CreateOrderResult>; 
    public record CreateOrderResult(Guid Id);
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator() 
        {
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName is requeired");
            RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("CustomerId is required");
            RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems ne moze prazno");
            // Samo sam ove kolone zeleo da validiram, a mogo sam i ostale. 
        }
    }
    // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom fajlu, zbog Clean architecture.
}
