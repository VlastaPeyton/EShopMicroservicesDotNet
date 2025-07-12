
using Basket.API.Data;
using Basket.API.DTOs;
using Basket.API.Extensions;
using Discount.gRPC;
using FluentValidation;

namespace Basket.API.Basket.StoreBasket
{   
    // Obasnjeno u CreateProductEndpoint i CreateProductCommandHandler. Da se ne ponavljam.
    public record StoreBasketCommand(StoreShoppingCartDTO Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);
 
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
    public class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {  /* IDocumentSession je DI u BasketRepository, jer Basket koristi Repository pattern da sva logika bude u tim metodama, a ne u Handler. Zbog Decorator tj CachedBasketRepository je decorator za BasketRepository, 
        ovaj IBasketRepository ukazivace na CachedBasketRepository.*/

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila Result object 
        u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            await DeductDiscount(command.Cart, cancellationToken); // Definisacu ispod ovu metodu
            
            await repository.StoreBasket(command.Cart.FromSCDtoToSC(), cancellationToken); // Koristi CachedBasketRepository metodu ali pre toga pretvori StoreShoppingCartDTO u ShoppingCart jer Repository treba da radi smo sa Models klasom.

            return new StoreBasketResult(command.Cart.UserName);
        }

        public async Task DeductDiscount(StoreShoppingCartDTO cart, CancellationToken cancellationToken)
        {
            // Komunicira sa Discout.gRPC i sracunaj cenu produkta posle popusta
            foreach (var item in cart.Items)
            {   /* GetDiscountAsync je kreiran automatski, jer u discount.proto ima GetDiscount i ovo vazi za svaku metodu u discount.proto file. 
                   GetDiscountRequest i da ima productName umesto ProductName, .NET automatski pravi PascalCase. 
                  Discount smo Seedovali samo za ProductName = "IPhone X" i "Samsung 10" . */
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}
