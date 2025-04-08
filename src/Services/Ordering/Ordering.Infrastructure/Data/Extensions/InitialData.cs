
using Ordering.Domain.Models;
using Ordering.Domain.Value_Objects;

namespace Ordering.Infrastructure.Data.Extensions
{   
    // Seeding podaci za Ordering tabele koji ce se koristiti u DatabaseExtensions.cs
    public class InitialData
    {   
        // Initial data za Customers tabelu
        public static IEnumerable<Customer> CustomersInitialData => new List<Customer>
        {
            Customer.Create(CustomerId.Of(new Guid("9cccebee-a305-4630-9e79-6722e14cd19c")), "mehmet", "mehmet@gmail.com"), // Customer1
            Customer.Create(CustomerId.Of(new Guid("11ecf20a-12e9-4131-8c24-659476b92f62")), "john", "john@gmail.com")      // Customer2
        };

        // Initial data za Products tabelu 
        public static IEnumerable<Product> ProductsInitialData => new List<Product>
        {
            Product.Create(ProductId.Of(new Guid("e5932196-713a-49c4-9f9a-6bb92304cccf")), "IPhone X", 500),
            Product.Create(ProductId.Of(new Guid("21a90a06-7908-4fd9-9cbd-12d9c025a51b")), "Samsung 10", 400),
            Product.Create(ProductId.Of(new Guid("394de0dc-bd7c-40f8-9ad3-f69284ba2b37")), "Huawei plus", 650),
            Product.Create(ProductId.Of(new Guid("310abd5a-d39d-4fdc-a075-df7aa6a4dae6")), "Xiaomi mi 9", 450)
            /* Ovo imamo u CatalogInitialData.cs, samo tamo Guid i cena drugaciji, ali ta klasa nam ne treba kad testiram Ordering. 
             Ta klasa treba kad testiram (Seedujem) catalogdb za Basket testiranje, jer za Basket nam treba da catalogdb ima products -nisam sig u ovo
              Proveri ovo da l trebad da se popiuni podacima iz CatalogInitialData.cs, ali ja msm da ne mora  */
        };
        
        // Initial data za Orders tabelu 
        public static IEnumerable<Order> OrderWithOrderItemsInitialData
        {
            get
            {
                var address1 = Address.Of("mehmet", "ozkaya", "mehmet@gmail.com", "Mehmet_ulica", "Turkey", "Istanbul", "38050");
                var address2 = Address.Of("john", "doe", "john@gmail.com", "John_ulica", "England", "Nottingam", "08050");

                var payment1 = Payment.Of("mehmet", "1111222233334444", "12/27", "355",1);
                var payment2 = Payment.Of("john",   "5555666677778888", "10/28", "700", 2);

                var order1 = Order.Create(
                    OrderId.Of(Guid.NewGuid()), // Sam generise Guid
                    CustomerId.Of(new Guid("9cccebee-a305-4630-9e79-6722e14cd19c")),
                    // Mora Guid kao Customer1 da bi imalo smisla
                    OrderName.Of("ORD_1"), // U OrderName.cs Of metodi uslov je 5 da bude duzina 
                    shippingAddress: address1,
                    billingAddress: address1,
                    payment1
                    );
                
                order1.AddOrderItem(ProductId.Of(new Guid("e5932196-713a-49c4-9f9a-6bb92304cccf")), 2, 500); // 2 komada IPhone X
                // Mora isti Guid  i cena kao neki od products iznad da bi imalo smila i isti
                order1.AddOrderItem(ProductId.Of(new Guid("21a90a06-7908-4fd9-9cbd-12d9c025a51b")), 3, 400); // 3 komada Samsung 10

                var order2 = Order.Create(
                    OrderId.Of(Guid.NewGuid()), // Sam generise Guid
                    CustomerId.Of(new Guid("11ecf20a-12e9-4131-8c24-659476b92f62")),
                    // Mora Guid kao Customer2 da bi imalo smisla
                    OrderName.Of("ORD_2"), // U OrderName.cs Of metodi uslov je 5 da bude duzina 
                    shippingAddress: address2,
                    billingAddress: address2,
                    payment1
                    );

                order2.AddOrderItem(ProductId.Of(new Guid("394de0dc-bd7c-40f8-9ad3-f69284ba2b37")), 2, 650); // 2 komada Huawei plus
                order2.AddOrderItem(ProductId.Of(new Guid("310abd5a-d39d-4fdc-a075-df7aa6a4dae6")), 2, 450); // 2 komada Xiaomi mi 9

                return new List<Order> { order1, order2 };
            }
        }
    }
}
