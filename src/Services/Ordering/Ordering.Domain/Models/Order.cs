using Ordering.Domain.Abstractions;
using Ordering.Domain.Enums;
using Ordering.Domain.Events;
using Ordering.Domain.Value_Objects;

namespace Ordering.Domain.Models
{   
    // Order je Aggregate(Aggregate Root) koji sadrzi Child Entities i Value Objects. Ovo je Orders tabela u bazi.
    public class Order : Aggregate<OrderId> 
    {
        /* Aggregate je abstract, ali nema metodu bez tela pa neam sta da override ovde. 
           Order nasledio sve iz Aggregate<OrderId> i Entity<OrderId> (jer Aggregate : Entity).
            
           Order ne moze da pristupi _domainEvents in Aggregate, jer je to private polje sa razlogom. Vec pristupa DomainEvents polju koje mu donosi  DomainEvents listu. 
           Order moze da pristupi DomainEvnets iz Aggregate jer je public. 

           Objasnjenje za _orderItems i OrderItems pogledaj u Aggregate.cs jer isti slucaj ima tamo.
        */
        private readonly List<OrderItem> _orderItems = new(); // Child Entity. Ovu listu menjamo po potrebi. Dok OrderItems sluzi da dohvatimo ovu listu bez mogucnosti menjanja van ove klase.
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();  // Navigational attribute, pa OnModelCreating moram definisati PK-FK vezu za Order-OrderItem, jer List<CustomType> ne moze biti polje Order tabele. Pristupam ovome pomocu Include u QueryHandler klasama jer je ovo Eager loading
       
        public CustomerId CustomerId { get; private set; } = default!; // FK. Povezuje Id of Customer.cs jer 1 Order pripada 1 Customer-u.
        public OrderName OrderName { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!; // Ovo je custom type property, pa u OnModelCreating tj OrderConfiguration moram rucno definisati ako zelim da polja iz ShippingAddress budu kolone Orders tabele
        public Address BillingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending; // U OnModelCreating tj OrderConfiguration.cs ako rucno ne namestim, u koloni Status Orders tabele stajace 1,2,3,4 umesto stringova
        // Status, Payment, Billing/ShippingAddres, OrderName su owned entities jer su custom type
        public decimal TotalPrice // Ako zelim da Expression-bodied property bude kolona Orders tabele, moram u OnModelCreating(tj u OrderConfiguration.cs) to rucno uraditi, dok za normalne property to ne vazi
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity); 
            private set { }
        }
        // Ova polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metodom van klase, dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
        // Zbog private set, moze new Order = {...} 

        // Polja su private set, pa ih moram setovati unutar klase konstruktorom ili static metod, dok nasledjeno Id polje ne moram, jer ono je public set u Entity.cs deifnisano.
        // Zbog Rich-domain dodajem Create static metodu umesto konstruktora kojom cu van klase moci da setujem      
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
            // OrderItems se dodaju pomocu AddOrderItem nakon Create 

            order.AddDomainEvent(new OrderCreatedEvent(order)); // Nasledjena metoda iz Aggregate koja modifikuje _domainEvents
            return order;
        }

        // Update Order without modify OrderItems list. Takodje nemogu azurirati Id jer se on ne azurira nikad.
        public void Update (OrderName orderName, Address shippingAddress, Address billingAddress, Payment payment, OrderStatus status)
        {
            /* Prosledjujem samo atribute koji se mogu azurirati. Nisam prosledio CustomerId i OrderItems, jer customer je taj koji kreira i modifikuje(ako zeli) svoj Order. 
            Naravno, moze se modifikovati i OrdeItems gde cu da izbrisem/dodam OrderItem iz Order(ShoppingCart), ali to cu uraditi u narednoj metodi. 
              Ova polja su Owned entities, i kad njih promenim, Interceptor to primeti pomocu AuditableEntityInterceptor (Infrastructure layer) i upise u Audit kolone of Order.*/

            OrderName = orderName;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Payment = payment;
            Status = status;

            AddDomainEvent(new OrderUpdatedEvent(this)); // Nasledjena metoda iz Aggregate
        }

        // Add new OrderItem 
        public void AddOrderItem(ProductId productId, int quantity, decimal price)
        {
            /* OrderItem ctor zahteva OrderId, ProductId, Quantity, Price. OrderId nisam naveo u potpisu ve metode, jer to je Id iz Order obzirom da Order:Aggregate<OrderId>, a Aggregate<OrderId>:Entity<OrderId>

            Validacija za quantiy i price jer su primary type tj nisu Value Object pa da imaju validaciju u svojoj klasi. 
            */
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var orderItem = OrderItem.Create(Id, productId, quantity, price); 

            _orderItems.Add(orderItem);  // reaonly List<OrderItem> dopusta modifikaciju liste, ali ne i pokazivaca na nju
        }

        // Remove OrderItem 
        public void RemoveOrderItem(ProductId productId)
        {
            // Na osnovu ProductId brisemo zeljeni OrderItem, jer ProductId u OrderItem odnosi se na Id u Product

            var orderItem = _orderItems.FirstOrDefault(x => x.ProductId == productId); // EF Core change tracker ne sme biti AsNoTracking jer ocu Remove(orderItem)
            // FirstOrDefault vrati null ako nije naso nijedan product i zato proveravamo sa if 
            if (orderItem is not null) 
                _orderItems.Remove(orderItem);
        }
    }
}
