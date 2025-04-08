using Basket.API.Data;
using FluentValidation;

namespace Basket.API.Basket.DeleteBasket
{    /* Prvo pogledaj DeleteBasketEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */

    public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
    public record DeleteBasketResult(bool IsSuccess);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */

    /* U Command(nikad u Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     ovo je strongly typed validaton from MediatR. Validacija je ista ona u BuildingBlocks koju Catalog koristio. */
    public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
    {    
        // Mora ovaj konstruktor
        public DeleteBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
    public class DeleteBasketCommandHandler (IBasketRepository repository)
        : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {   /* IBasketRepository, a ne BasketRepositor, jer znamo da uvek interface ide, a u Program.cs cu da registrujem
         da prepoznaje IBasketRepository kao BasketRepository. */

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila Result object 
       u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {   
            await repository.DeleteBasket(command.UserName, cancellationToken);

            return new DeleteBasketResult(true);
        }
    }
}
