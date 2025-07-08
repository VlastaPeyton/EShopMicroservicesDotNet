namespace Catalog.Api.Models
{   
    // Catalog service ima Vertical slice arhitekturu, pa svi layers su unutar istog projekta under Catalog folder. Da je Clean architecture, Models folder bi bio Domain layer posebno, a ovde je unutar Catalog foldera Domain layer.
    public class Product // Product entity  tj kolone tabele u bazi
    {
        public Guid Id { get; set; } // Zbog imena, automatski ce baza znati da je ovo PK. Guid je najbolji type za Id. Ne sme imati default value.
        public string Name { get; set; } = default!; 
        // default da bude default vrednost ako ne definisem prilikom kreiranja objekta. 
        // ! - compiler nece prikazati upozorenje da vrednost mora biti inicijalizovana
        public List<string> Category { get; set; } = new();
        // Product-Categori veza je 1-to-many, jer 1 Product moze biti u vise kategorija
        // Category kolona postojace u tabeli, jer string nije object type. Da je bilo List<Klasa>, ovo bi bio navigation attribute koji bih u LINQ morao sa Include da dohvatim ako zelim.
        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default!;
        public Decimal Price { get; set; } // Ne sme imati default zbog logike poslovanja.
    }
}
