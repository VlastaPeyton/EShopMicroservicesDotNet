using Basket.API.Exceptions;
using Marten;

namespace Basket.API.Data
{
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {   /* Kao u Catalog, u Program.cs registrovacu LightWeightSession za Marten IDocumentSession 
         i onda ce BasketRepository znati da se na to session odnosi na DocumentSession. Kao sto znam 
        iz Catalog, IDocumentSession je isto kao DbContext (koji se koristi za SQL) samo za NoSQL. Zbog NoSQL
        baze, nemamo ime tabele, bas kao kod Catalog, iako ovde koristim Repository pattern. 
        Session, kao DbContext, u sebi sadrzi comit i rollback. */
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);
            /* LoadAsync<ShoppingCart> nadje tabelu tipa ShoppingCart i na osnovu datom userName nadje zeljeni basket
            U Program.cs namesteno da UserName polje iz ShoppingCart bude PK jer ta klasa nema Id polje koje inace je PK 
            a moralo je neko polje biti PK. */
            return basket is null ? throw new BasketNotFoundException(userName) : basket;

        }
        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            /*If ShoppingCart exist in DB, Marten will update it. If ShoppingCart not exist, Marten will insert it. 
             Store(basket) na osnovu typeof(basket)=ShoppingCart nadje tabelu tog tipa i upise u nju 
             Kod Catalog, imao sam CreateProduct (session.Store(product)) i UpdateProduct(session.Update(product)), 
            ali ovde necu da radim session.Update, jer izgleda da moze i samo preko session.Store(). Da sam ovo znao kod 
            Catalog, ne bih pravio dva featura, vec samo jedan sa session.Store(product). */
            session.Store(basket); 
            await session.SaveChangesAsync();
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {   
            // Delete<ShoppingCart> nadje tabelu tipa ShoppingCart i izbrise vrstu za dati userName
            session.Delete<ShoppingCart>(userName); 
            await session.SaveChangesAsync();
            return true;
        }
    }
}
