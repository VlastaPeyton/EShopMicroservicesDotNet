using Ordering.Domain.Abstractions;
using Ordering.Domain.Enums;
using Ordering.Domain.Events;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // Order je Aggregate(Aggregate Root) koji sadrzi Child Entity i Value Objects
    public class Order : Aggregate<OrderId> // Orders tabela u bazi, jer nasledio Aggregate(Entity)
    {
        /* Aggregate je abstract, ali nema metodu bez tela pa neam sta da override ovde. 
        Order nasledio sve iz Aggregate<OrderId> i Entity<OrderId> (jer Aggregate : Entity), pa Id(OrderId tipa),CreateAt,
        ModifiedOn, ModifiedBy, CreatedBy polja iz Entity su nasledjena. 
           
           Nasledjeno Id(OrderId tipa) polje predstavlja OrderId. 
        */
        private readonly List<OrderItem> _orderItems = new(); // Child Entity
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();  // OrderItems je tabela 
        // Objasnjenje za ove 2 linije iznad lezi u Aggregate.cs 

        public CustomerId CustomerId { get; private set; } = default!; // Povezuje Id of Customer.cs jer 1 Order pripada 1 Customer-u. Custoemrs je tabela 
        public OrderName OrderName { get; private set; } = default!;
        // Ocu da CustomerId i OrderName budu strongly typed polja jer to je dobra praksa

        // Address i Paymetn je Value Object jer to je custom klasa bez Id polja koju cu definisati
        public Address ShippingAddress { get; private set; } = default!;
        public Address BillingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;

        //OrderStatus je enum koji cu definisati
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity); // OrderItem.cs imace Price i Quantity 
            private set { }
        }

        /* Polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodo
       dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
         Zbog Rich-domain dodajem Create static metodu umesto ktora gde cu i Id da popunim.   
       */
        public static Order Create (OrderId orderId, CustomerId customerId, OrderName orderName,
                                    Address shippingAddress, Address billingAddress, Payment payment)
        {
            // Status nisam ubacio u zaglavlje metode, jer Enum nikad ne ide u zaglavlje kad se kreira order
            // Ne validiramo jer svaki Value Objcet (CustomerId, OrderName, Address, Payment) ima validaciju 
            var order = new Order
            {
                Id = orderId, // Nasledio iz Aggregate (tj iz Entity)
                CustomerId = customerId,
                OrderName = orderName,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                Payment = payment,
                Status = OrderStatus.Pending
            };

            order.AddDomainEvent(new OrderCreatedEvent(order)); // Nasledjena metoda iz Aggregate
            // Definisacu kasnije ovu klasu

            return order;
        }

        // Update Order without modify OrderItems list
        public void Update (OrderName orderName, Address shippingAddress, Address billingAddress, 
                            Payment payment, OrderStatus status)
        {
            /* Prosledjujem samo atribute koji se mogu azurirari. Nisam prosledio CustomerId i OrderItems 
             jer customer je taj koji  kreira i modifikuje(ako zeli) svoj Order. Naravno, moze se modifikovati
            i OrdeItems gde cu da izbrisem/dodam OrderItem iz Order(ShoppingCart), ali to cu uraditi u narednoj metodi*/

            OrderName = orderName;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Payment = payment;
            Status = status;

            AddDomainEvent(new OrderUpdatedEvent(this)); // Nasledjena metoda iz Aggregate
            // Definisacu kasnije ovu klasu 
        }

        // Add new OrderItem 
        public void AddOrderItem (ProductId productId, int quantity, decimal price)
        {   
            /* OrderItem ctor zahteva OrderId, ProductId, Quantity, Price. OrderId se u OrderItem odnosi na Id u Order,
            jer Order : Aggregate<OderId>. 
            Prosledio sam ProductId, Quantity, Price, jer to zahteva OrderItem ctor, ali Id(orderId) nisam prosledio, jer 
            to imam posto Order : Aggregate<OrderId> pa je Order nasledio Id tipa OrderId.

            Validacija za quantiy i price jer su primary type tj nisu Value Object pa da imaju validaciju u svojoj klasi */
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var orderItem = new OrderItem(Id, productId, quantity, price);
            
            _orderItems.Add(orderItem); // Add je built-in za listu
            /* Kad modifikujem OrderItems listu, moram preko _orderItems, jer se ta lista moze modifikovati, dok 
            OrderItems se ne moze modifikovati jer je IReadOnly lista. */
        }

        // Remove OrderItem 
        public void RemoveOrderItem (ProductId productId)
        {
            /* Na osnovu ProductId brisemo zeljeni OrderItem, jer ProductId u OrderItem odnosi se na Id u Product
            jer Product:Entity<ProductId> */

            var orderItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
            // _orderItems je tipa OrderItem, a svaki OrderItem ima ProductId
            // FirstOrDefault vrati null ako nije naso nijedan product i zato proveravamo sa if 
            if (orderItem != null) 
                _orderItems.Remove(orderItem);
        }
    }
}
