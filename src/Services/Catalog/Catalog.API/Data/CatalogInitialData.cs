using Marten.Events;
using Marten.Schema;

namespace Catalog.API.Data
{
    /* Koristim za testiranje StoreBasket Endpoint u Postman, jer moram da imam tad neke products
     kako bi mogo Basket da ih selektuje. 
       IInitialData is from Marten */
    public class CatalogInitialData : IInitialData
    {   
        // Moram ovu metodu zbog interface i to async 
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();
            /*If there is any product u tabelu koja, zbog nemanja Repository pattern, nema ime
            ali je Product tipa i Query<Product> nadje tabelu tog tipa i sa AnyAsync proveri ima li bar 
            1 vrsta tabeli. */
            if (await session.Query<Product>().AnyAsync())
                return; // Izlazim iz Populate, jer tabela nije prazna i nema potrebe da Seedujem

            // Ako tabela Product type prazna, Seedujem produckte koje cu ispod definisati
            session.Store<Product>(GetPreconfiguredProducts());
            /* moze i bez <Product> jer GetPreconfiguredProducts vraca List<Product> pa Marten sam provali 
             u koju tabelu da upise */

            await session.SaveChangesAsync();
        }

        private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
        {
            new Product()
            {   // Ovako pravimo product klasu, jer nema explicitni konstruktor 

                // Vodi racuna da Guid ima ovakav oblik. Na netu nadji Guid generator da izlupa ove brojeve
                Id = new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"),
                Name = "IPhone X",
                Description = "IPhone X description",
                ImageFile = "iphonex.png",
                Price = 950.00M,
                Category = new List<string> {"Smart Phone"}
            },

            new Product()
            {
                Id = new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"),
                Name = "Samsung 10",
                Description = "Samsung 10 description",
                ImageFile = "samsiung10.png",
                Price = 840.00M,
                Category = new List<string> {"Smart Phone"}
            },

            new Product()
            {
                Id = new Guid("4f136e9f-ff8c-41cf-9a33-d12f689bda8"),
                Name = "Huawei plus",
                Description = "Huawei plus descripiton",
                ImageFile = "huaweiplus.png",
                Price = 650.00M,
                Category = new List<string> {"White Appliances"}
            },

            new Product()
            {
                Id = new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"),
                Name = "Xiaomi mi 9",
                Description = "XIaomi mi 9 descripiton",
                ImageFile = "hiaomimi9.png",
                Price = 470.00M,
                Category = new List<string> {"White Appliances"}
            },

            new Product()
            {
                Id = new Guid("b786103d-c621-4f5a-b498-23452610f88c"),
                Name = "HTC U11+",
                Description = "HTC U11+ descripiton",
                ImageFile = "htcu11+.png",
                Price = 470.00M,
                Category = new List<string> {"Camera"}
            },


        };
    }
}
