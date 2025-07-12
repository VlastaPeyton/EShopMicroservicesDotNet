namespace Catalog.API.DTOs
{   
    // DTO klasa se koristi da razdvoji Domain od Application/API layera jer ne valja da koristim Models klase obzirom da predstavljaju tabele u bazi.
    public class ProductResultDTO
    {   
        // Imace sva polja kao Product.cs jer mi sve treba. Osim Id, jer to Clientu nikad ne treba.
        public string Name { get; set; } = default!;
        public List<string> Category { get; set; } = new();
        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default!;
        public Decimal Price { get; set; }
    }
}
