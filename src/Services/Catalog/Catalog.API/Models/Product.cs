namespace Catalog.Api.Models
{   
    // Catalog service ima Vertical slice arhitekturu, pa svi layers su unutar istog projekta under Catalog folder. Da je Clean architecture, Models folder bi bio Domain layer posebno, a ovde je unutar Catalog foldera Domain layer.
    // Catalog koristi NoSQL bazu (Postgres via Marten,a Marten je pandan EF Core). U NoSQL, Tabela se zove Collection, Row se zove Document, Column se zove Field.
    // NoSQL DB nema PK-FK, ali ima PK koji je Id autoamtski ako to polje postoji, ako ne postoji, moram u Program.cs da navedem koje zelim da bude PK.
    public class Product // Ovo nije Entity jer koristim Marten zbog Postgre NoSQL, a ne EF Core zbog SQL baze. ALi svakako predstavlja "tabelu" u bazi tipa Product
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
