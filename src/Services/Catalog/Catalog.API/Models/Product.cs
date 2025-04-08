namespace Catalog.Api.Models
{
    public class Product // Product entity
    {
        public Guid Id { get; set; } // Zbog imena, automatski ce baza znati da je ovo PK
        public string Name { get; set; } = default!;
        /* default! znaci da compiler ne izbaci upozorenje ako nije siguran 
         da cemo explicitno iniicjalizovati sa non null vrednst ovo polje. 
           
           Product has 1-to-many relationship with Category, jer 1 Product moze 
        pripadati u vise kategorija proizvoda. */
        public List<string> Category { get; set; } = new();
        // new() = new List<string>()

        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default;

        public Decimal Price { get; set; }

        /* Nisam definisam konstruktor, stoga mogu uraditi sledece prilikom setovanja 
         polja, jer sva polja su public set.
           1) product = new Product();
              product.Id = Guid.NewGuid()";
              ....
              product.Price = 10.5; 
            
           2) product = new Product 
              {
                Id = Guid.NewGuid();
                ...
                Price = 10.5;
              };
            
            Id polje smo ovako definisali, jer kad napravimo u (Postgre NoSQL) bazi tabelu 
        cije ce kolone biti ova polja iz Products, Id ce se samo generisati, bez da radimo rucno
        kao u ova dva primera iznad.
             */
    }
}
