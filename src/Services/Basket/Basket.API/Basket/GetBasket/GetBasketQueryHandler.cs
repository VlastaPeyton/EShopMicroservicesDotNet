
using Basket.API.Data;
using Basket.API.DTOs;
using Basket.API.Extensions;

namespace Basket.API.Basket.GetBasket
{   // Pogledaj StoreBasketEndpoint i StoreBasketCommandHandler. Da ne ponavljam.

    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
    public record GetBasketResult(ReturnShoppingCartDTO cart);
    public class GetBasketQueryHandler (IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
    {  // Pogledaj StoreBasketCommandHandler objasnjeno za repository zasto korisit CachedBasketRepository.
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasket(query.UserName, cancellationToken); // ShoppingCart
        
            return new GetBasketResult(basket.ToReturnShoppingCartDTO()); // Jer klijentu treba DTO da vratim, a ne Domain klasu
        }
    }
}
