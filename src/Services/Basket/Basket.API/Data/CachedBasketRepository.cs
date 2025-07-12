using System.Text.Json; // Mogo sam i JSON NewtonSoft Nuget Package umesto ovoga isto je
using Microsoft.Extensions.Caching.Distributed; // For IDistributedCache 

namespace Basket.API.Data
{
    /* Proxy and Decorator pattern for Redis:
     
       1)Proxy pattern: CachedBasketRepository acts as a proxy tj forwarding calls to underlying 
    BasketRepository. Omogucava lazy loading. 

       2)Decorator pattern: Excend functionality of BasketRepository by adding cache logic sto znaci
    da ova klasa mora da implementira IBasketRepository bas kao sto je i BasketRepository morala. Dodavanjem CachedBasketRepository kao Decorator za BasketRepository, u Handler klasama IBasketRepository predstavljace CachedBasketRepository. 

       
       Redis (cache) radi tako sto umesto da citamo svaki put iz baze nesto, prvo da l to ima u cache, ako ima onda odatle uzme sto je bre nego iz baze, a ako nema, onda iz baze upisujemo u cache, pa onda 
    dohvatamo iz cache. AKo upisujemo u bazu, prvo upisemo u bazu, pa onda u cache.
    
       Redis sve cuva u key-value formatu, gde value mora biti JSON oblika. 
    
    */

    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        /* CachedBasketRepository mora da implementira IBasketRepository, bas kao BasketRepository sto je uradio, ali posto CachedBasketRepository je decorator za BasketRepository tj koristi BasketRepository 
         + dodaje svoje (Cache), moram da uvezem BasketRepository, a to cu pomocu IBasketRepository, jer u Program.cs registrovao IBasketRepository kao BasketRepository. 
           
           IDistributedCache registrovan u Program.cs kao AddStackExchangeRedisCache cime konektujem Redis in Docker with this code
         */

        // Ovo su metode iz IBasketRepository koje moram da definisem, jer ga CachedBasketRepository implementira => BasketRepositor i CachedBasketRepository imaju iste potpise metoda
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken)
        {   
            // Prvo, provera da li to sto trazim je u cache
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
            
            // Ako ima u cache, onda vrati ga clientu
            if (!string.IsNullOrEmpty(cachedBasket))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!; // Napravi ShoppingCart from JSON jer Redis samo JSON sadrzi

            // Ako nema u cache, uzimamo iz baze
            var basket = await repository.GetBasket(userName, cancellationToken); // ReturnShoppingCartDTO koji vracam klijentu, a ne ShoppingCart vec ReturnShoppingCartDTO zbog razdvajanja Domain i API layera
            // Pa upisujemo u cache da ima za naredni put ako zatreba. 
            // Redis cuva sve u key-value formatu, gde value mora biti JSON i zato basket pretvaramo u JSON, dok key je userName jer je prosledjen GetBasket metodi. 
            await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
            // Pa vratimo clientu ali ne ShoppingCart vec ReturnShoppingCartDTO zbog razdvajanja Domain i API layera
            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
        {   
            // Prvo, stavim u bazu
            await repository.StoreBasket(basket, cancellationToken);
            // Onda, stavim i u cache
            //Redis cuva sve u key-value formatu, gde value mora biti JSON i zato basket pretvaramo u JSON, dok key je userName jer je prosledjen StoreBasket metodi.
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken)
        {
            // Prvo, obrisem iz baze
            await repository.DeleteBasket(userName, cancellationToken);
            // Onda, obrisem iz cache
            /* Prosledimo mu key tj userName i obrise value(JSON) koji pripada tom kljucu*/
            await cache.RemoveAsync(userName, cancellationToken);
            return true;
        }
    }
}
