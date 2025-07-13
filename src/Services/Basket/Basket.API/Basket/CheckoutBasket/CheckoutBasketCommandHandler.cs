using Basket.API.Data;
using Basket.API.DTOs;
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket
{   // Objasnjeno u CheckoutBasketEndpoint, StoreBasketEndpoint i StoreBasketCommandHandler
    public record CheckoutBasketCommand(BasketCheckoutDTO BasketCheckoutDto) : ICommand<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);

    public class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketValidator()
        {
            RuleFor(x => x.BasketCheckoutDto).NotEmpty().WithMessage("BasketCheckoutDto ne moze null");
            RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("Username ne moze prazna");
        }
    }
    public class CheckoutBasketCommandHandler(IBasketRepository repository, IPublishEndpoint publishEndpoint) : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {      
        //  IPublishEndpoint je pandan IMediator u DispatchDomainEventInterceptor.cs (koji publish DomainEvent u Ordering) */

        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            // Get existing Basket with total price 
            var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
            if (basket is null)
                return new CheckoutBasketResult(false);

            // Create BasketCheckoutEvent message and set total price
            var integrationEvent = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>(); // BasketCheckoutEvent je definisan u BB.Messaging
            // Mapiranje moze jer BasketCheckoutDTO ima polja istog imena i tipa kao BasketCheckoutEvent 
            integrationEvent.TotalPrice = basket.TotalPrice; 

            // Publish BasketCheckoutEvent to RabbitMQ using MassTransit
            await publishEndpoint.Publish(integrationEvent);

            // Delete Basket after checkout
            await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken); 

            // Zadnje dve linije iznad interaguju sa 2 sistema (RabitMQ i DB) i to je Dual-write problem koji resavam posle ako budem znao

            return new CheckoutBasketResult(true);
        }
    }
    
}
