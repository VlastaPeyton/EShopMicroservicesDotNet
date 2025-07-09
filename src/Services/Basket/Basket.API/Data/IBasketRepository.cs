namespace Basket.API.Data
{   
    /* Interface uvek radim za Repository, zbog DI, jer u Handler koristicu IBasketRepository, dok u Program.cs registrovacu IBasketRepository kao BasketRepository.
       Catalog ima skoro sve isto kao Basket, ali nema Repository, pa sam tamo u Handle metodama definisao logiku za pristu bazi. U Basket, tu logiku stavim u BasketRepository
    metode koje su u IBasketRepoisotry samo potpisane. U Catalog svakom Handler uvozio sam IDocumentSession, ali ovde cu IDocumentSession da uvezem u BasketRepository. 
    Kao i kod Catalog, koristicu LightWeightDocumentSession koji nema automatski Change Tracker, pa moram pisati session.Store/Update ako zelim da promena ostane u bazi nakon SaveChangesAsync.
    Pogledaj CreateProductEndpoint i CreateProductCommandHandler sve za IDocumentSession pise kako je to pandan DbContext za NoSQL via Marten i da postoji commit/rollback unutar session + moze LINQ, ali
    nema Change Tracker, pa mora rucno session.Store/Update before SaveChangesAsync ako zelim da azuriram bazu.
       
       Obzirom da koristim i Redis cache, napravicu CachedBasketRepository, koji ce u Program.cs biti registrovan kao Decorator for BasketRepository tj dodace Cache na BasketRepository. Mogo sam da nemam
    BasketRepository, vec da svu njegovu logiku upisem u CachecBasketRepository ali to nije dobra praksa.  Dodavanjem CachedBasketRepository kao Decorator za BasketRepository, u Handler klasama IBasketRepository predstavljace CachedBasketRepository.
    */
    public interface IBasketRepository
    {
        // Sve metode bice async Task<T> u BasketRepository, pa ovde im pisem Task<T> 
        // CancellationToken nema =default jer u Handler ,koji poziva ove metode, ne stoji =default i to je jako bitno jer ako stavim =default, necu moci da prekinem ove metode. Prosledjujem ga svim async metodama.
        // Metode imaju userName argument, jer ShoppingCart.cs je Basket, a u Program.cs definisano UserName polje kao PK i onda LoadAsync/Delete u GetBasket/DeleteBasket mogu zbog PK
        Task<ShoppingCart> GetBasket (string userName, CancellationToken cancellationToken);
        Task<ShoppingCart> StoreBasket (ShoppingCart basket, CancellationToken cancellationToken);
        Task<bool> DeleteBasket (string userName, CancellationToken cancellationToken);
    }
}
