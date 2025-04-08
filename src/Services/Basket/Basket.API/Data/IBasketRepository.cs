namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        /* U Repository, definisacu po jednu async metodu za svaki Basket feature (GetBasket, StoreBasket, DeleteBasket), zato sam
         i ostavio sve Handle metode iz Query/CommandHandler klasa da treba da im se definise telo, e pa sad cu to 
         uraditi, ali ne u Handle metodi, vec u Repository metodama. Ovime se postize Repository pattern koji nema kod Catalog. 
        U Catalog sam za svaku Handler klasu imao IDocumentSession (IDocumentSession se koristi zbog Marten jer pretvara
        PostgreSQL u NoSQL), pa modifikovanje/ocitavanje baze radilo u Handle metodi. U Basket, zbog Repository pattern, 
        to necu tako, vec IDocumentSession se stavi u BasketRepository (koji ce da implementira IBasketRepository), dok
        Handler klasa imace samo IBasketRepository. Modifikovacu Handle metode da pozivaju repository metode GetBasket, StoreBasket i 
        DeleteBasket,respektivno, a pozivace ih pomocu BasketRepository, jer ce Handler klase imati IBasketRepository (kroz 
        Primary Construktro recimo ubacen DI). Da bi Handler klase znale da se IBasketRepository odnosi na BasketRepository, 
        u Program.cs cu to registrovati. Cao kod Catalog, znamo da u Program.cs registrujem IDocumentSession kao LightWeightDocumentSession
        
           Rezime: Catalog i Basket imaju Vertical Slice arhitek, ali Catalog nema Repository pattern, pa mu je upis/citanje baze
        u Handle metodi, dok Basket ima Repository pattern, pa mu je ta logika u BasketRepository GetBasket, StoreBasket i DeleteBasket 
        metodama smestena, a Handle metode ce samo da pozivaju GetBasket, StoreBasket i DeleteBasket metode. */
        
        // Ove metode su async -> Task<...> jer sam u interface, dok u BasketRepository bice async Task<...>

        // GetBasket metoda za GetBasket feature - string userName, jer GetBasketQuery(string UserName)
        Task<ShoppingCart> GetBasket (string userName, CancellationToken cancellationToken = default);

        // StoreBasket metoda za StoreBasket feature - ShoppingCart basket, jer StoreBasketCommand(ShoppingCart Cart)
        Task<ShoppingCart> StoreBasket (ShoppingCart basket,  CancellationToken cancellationToken = default);

        // Deletebasket metoda za DeleteBasket feauture - string userName, jer DeleteBasketCommand(string UserName)
        Task<bool> DeleteBasket (string userName, CancellationToken cancellationToken = default);
    }
}
