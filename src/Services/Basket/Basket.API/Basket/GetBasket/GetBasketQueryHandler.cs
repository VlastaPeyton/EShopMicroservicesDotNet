
using Basket.API.Data;

namespace Basket.API.Basket.GetBasket
{   // Pogledaj StoreBasketEndpoint i StoreBasketCommandHandler. Da ne ponavljam.

    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCart Cart);
    public class GetBasketQueryHandler (IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
    {  // Pogledaj StoreBasketCommandHandler objasnjeno za repository zasto korisit CachedBasketRepository.
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasket(query.UserName, cancellationToken);
       
            return new GetBasketResult(basket);
            // Jer ShoppingCart ima konstruktor koji zahteva UserName
        }
    }
}
