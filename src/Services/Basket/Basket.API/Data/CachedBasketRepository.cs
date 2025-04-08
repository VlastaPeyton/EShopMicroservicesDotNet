using System.Text.Json; // Mogo sam i JSON NewtonSoft Nuget Package umesto ovoga isto je
using Microsoft.Extensions.Caching.Distributed; // For IDistributedCache 

namespace Basket.API.Data
{
    /* Proxy and Decorator pattern for Redis. 
       1)Proxy pattern: CachedBasketRepository acts as a proxy tj forwarding calls to underlying 
    BasketRepository. Omogucava lay loading. 
       2)Decorator pattern: Excend functionality of BasketRepository by adding cache logic sto znaci
    da ova klasa mora da implementira IBasketRepository bas kao sto je i BasketRepository mora.
       
       Redis (cache) radi tako sto umesto da citamo svaki put iz baze nesto, mi vidimo da l to ima u cache,
    ako ima onda odatle uzimamo sto je bre nego iz baze, a ako nema, onda iz baze upisujemo u cache, pa onda 
    dohvatamo iz cache. AKo upisujemo u bazu, prvo upisemo u bazu, pa onda i u cache.
    
       Redis sve cuva u key-value formatu, gde value mora biti JSON oblika. */

    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
        : IBasketRepository
    {
        /* Posto IBasketRepository je registrovan u Program.cs, isto uradicu i za IDistributedCache.
           Iako IBasketRepository stoji u Primary Constructor, to ne znaci da ga ova klasa implementira
        (vec tako samo pravimo repository atribut ) i zato moram da ga implementiram kako bih mogo da 
        definisem  metode iz IBasketRepository i zbog Program.cs  u kome cu da uradim 
        builder.Services.Decorate<IBasketRepository, CachedBasketRepository>() bas kao sto sam i  
        builder.Services.AddScoped<IBasketRepository, BasketRepository>() za repository. */
        
        // Ovo su metode iz IBasketRepository koje moram da definisem, jer ga implementira ova klasa
        
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {   
            // Prvo, provera da li to sto trazim je u cache
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
            
            // Ako ima u cache, onda vrati ga clientu
            if (!string.IsNullOrEmpty(cachedBasket))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

            // Ako nema u cache, uzimamo iz baze
            var basket = await repository.GetBasket(userName, cancellationToken);
            // Pa upisujemo u cache da ima za naredni put ako zatreba. 
            /* Redis cuva sve u key-value formatu, gde value mora biti JSON i zato basket pretvaramo u JSON.
            dok key je userName jer je prosledjen GetBasket metodi. */
            await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
            // Pa vratimo clientu
            return basket;
        
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {   
            // Prvo, stavim u bazu
            await repository.StoreBasket(basket, cancellationToken);

            // Onda, stavim i u cache
            /* Redis cuva sve u key-value formatu, gde value mora biti JSON i zato basket pretvaramo u JSON.
            dok key je userName jer je prosledjen StoreBasket metodi. */
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
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
