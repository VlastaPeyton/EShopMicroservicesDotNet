using Basket.API.Exceptions;
using Marten;

namespace Basket.API.Data
{   
    // U IBasketRepository objasnjeno sve. Cancellation token prosledim svim async metodama.
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {   
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken)
        {
            var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken); // Moze LoadAsync bu UserName, jer UserName u Program.cs namesten kao PK.
            /* LoadAsync<ShoppingCart> nadje tabelu tipa ShoppingCart i na osnovu  userName nadje zeljeni basket
            U Program.cs namesteno da UserName polje iz ShoppingCart bude PK jer ta klasa nema Id polje koje inace je PK automatki a moralo je neko polje biti PK. */
            return basket is null ? throw new BasketNotFoundException(userName) : basket;

        }
        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
        {
            /*If ShoppingCart exist in DB, Marten will update it. If ShoppingCart not exist, Marten will insert it. 
             Store(basket) na osnovu typeof(basket)=ShoppingCart nadje tabelu tog tipa i upise u nju 
             Kod Catalog, imao sam CreateProduct (session.Store(product)) i UpdateProduct(session.Update(product)), 
            ali ovde necu da radim session.Update, jer moze preko session.Store() da se update. Da sam ovo znao kod 
            Catalog, ne bih pravio dva featura, vec samo jedan sa session.Store(product). */
            session.Store(basket);  // Mora jer LightWeightDocumentSession nema Change Tracker, pa moram rucno pre SaveChangesAsync uraditi ovo
            await session.SaveChangesAsync(cancellationToken);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken)
        {   
            // Delete<ShoppingCart> nadje tabelu tipa ShoppingCart i izbrise vrstu za dati userName
            session.Delete<ShoppingCart>(userName); // UserName definisan kao PK i zato ovo moze
            await session.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
