
using Basket.API.Data;
using Discount.gRPC;
using FluentValidation;

namespace Basket.API.Basket.StoreBasket
{   /* Prvo pogledaj StoreBasketEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */

    /* U Command(nikad u Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     ovo je strongly typed validaton from MediatR. Validacija je ista ona u BuildingBlocks koju Catalog koristio. */
    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        // Mora ovaj konstruktor
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Shopping Cart cannot be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
            // Mogao sam validirati jos polja, osim UserName, iz ShoppingCart klase, ali necu. 
        }
    }
    public class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) 
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {   /* IBasketRepository, a ne BasketRepositor, jer znamo da uvek interface ide, a u Program.cs cu da registrujem
         da prepoznaje IBasketRepository kao BasketRepository.
          DiscountProtoService.DiscountProtoServiceClient moze jer sam dodao Client gRPC na Basket.API i selektovao discount.proto iz Discount, 
        jer Discount je gRCP Server.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila Result object 
        u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            await DeductDiscount(command.Cart, cancellationToken); // Definisacu ispod ovu metodu
            
            await repository.StoreBasket(command.Cart,cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }

        public async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
        {
            // Komunicira sa Discout.gRPC i sracunaj cenu produkta posle popusta
            foreach (var item in cart.Items)
            {   /* GetDiscountAsync je kreiran automatski, jer u discount.proto ima GetDiscount i ovo vazi za svaku
                 metodu u discount.proto file. 
                   GetDiscountRequest i da ima productName umesto ProductName, .NET automatski pravi PascalCase. 
                  Discount smo Seedovali samo za ProductName = "IPhone X"/"Samsung 10" . */
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}
