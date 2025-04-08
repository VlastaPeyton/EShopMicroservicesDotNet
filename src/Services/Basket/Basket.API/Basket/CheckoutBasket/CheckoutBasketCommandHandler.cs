using Basket.API.Data;
using Basket.API.DTOs;
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket
{    /* Prvo pogledaj CheckoutBasketEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    
    public record CheckoutBasketCommand(BasketCheckoutDTO BasketCheckoutDto) : ICommand<CheckoutBasketResult>;
    /* Koristim BasketCheckoutDTO, jer Ordering (Integration Event Subscriber) ima Clean architecture gde razdvajam Application/API layer od Domain + 
     ovime se postize sigurnost. BasketCheckoutDTO ima ista polja kao BasketCheckoutEvent.cs  */
    public record CheckoutBasketResult(bool IsSuccess);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
    Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */

    /* U Command(nikad u Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     ovo je strongly typed validaton from MediatR. Validacija je ista ona u BuildingBlocks koju Catalog koristio. */

    public class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketValidator()
        {
            RuleFor(x => x.BasketCheckoutDto).NotEmpty().WithMessage("BasketCheckoutDto ne moze null");
            RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("Username ne moze prazna");
        }
    }
    public class CheckoutBasketCommandHandler(IBasketRepository repository, IPublishEndpoint publishEndpoint)
        : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {   /* IBasketRepository, a ne BasketRepositor, jer znamo da uvek interface ide, a u Program.cs cu da registrujem
         da prepoznaje IBasketRepository kao BasketRepository.  
           
           IPublishEndpoint je pandan IMediator u DispatchDomainEventInterceptor.cs koja publish DomainEvent u Ordering */

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila Result object 
       u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            // Get existing Basket with total price 
            var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
            if (basket is null)
                return new CheckoutBasketResult(false);

            // Create BasketCheckoutEvent message and set total price
            var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>(); // Def u BB.Messaging
            // Mapiranje moze jer BasketCheckoutDTO ima polja istog imena i tipa kao BasketCheckoutEvent 
            eventMessage.TotalPrice = basket.TotalPrice; 

            // Publish BasketCheckoutEvent to RabbitMQ using MassTransit
            await publishEndpoint.Publish(eventMessage); // IPublishEndpoint je pandan IMediator u DispatchDomainEventInterceptor.cs

            // Delete Basket after checkout
            await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken).ConfigureAwait(false); 

            // Zadnje dve linije iznad interaguju sa 2 sistema (RabitMQ i DB) i to je Dual-write problem koji resavam posle

            return new CheckoutBasketResult(true);
        }
    }
    
}
