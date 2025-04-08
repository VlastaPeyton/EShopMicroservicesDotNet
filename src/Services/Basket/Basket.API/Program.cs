using Basket.API.Data;
using BuildingBlocks.Behaviours;
using BuildingBlocks.Exceptions;
using Discount.gRPC;
using HealthChecks.UI.Client;
using Marten;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;
var builder = WebApplication.CreateBuilder(args);

// Add servicess to the container 

var assembly = typeof(Program).Assembly;
//Add Carter 
builder.Services.AddCarter();
// Add MediatR
builder.Services.AddMediatR(config =>
{   // Registruje Handler klase
    config.RegisterServicesFromAssemblies(assembly);
    // Dodaj Validation (def u BuildingBLocks) in Mediatr pipeline za Handler
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    // Dodaj Logging (def u BuldingBlocks) u MediatR pipeline za Handler 
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
}); 

// Add Marten for Postgres NoSQL
builder.Services.AddMarten(config =>
{   // Citanje ConnectionStrings za bazu iz appsettings
    config.Connection(builder.Configuration.GetConnectionString("Database")!);
    /* ShoppingCart.cs nema Id polje, pa uzimamo UserName polje za PK u tabeli koja je 
    ShoppingCart tipa(jer NoSQL tabela nema ime). */
    config.Schema.For<ShoppingCart>().Identity(x => x.UserName);

}).UseLightweightSessions(); // Kao kod Catalog koristim LightWeighSession jer je dobar
 
// Add Repository as DI da Handler klase prepoznaju IBasketRepository kao BasketRepository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
/* Pored ovog iznad, moram da dodam i Redis. AKo uradim AddScoped<IBasketRepository, CachedBasketRepository>()
 onda cu da override ovo iznad. Ako ne koristim Scrutor, moram uraditi sledece da izbegnem override
  
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var basketRepository = provider.GetService<BasketRepository>();
    return new CachedBasketRepository(basketRepository, provider.GetRequiredServices<IDistributedCache>());
});

   Ali ovo nije dobro, jer ogroman kod. Vec radim with Scrutor library kao ispod.
*/
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

// Add IDistributedCache fro StackExchangeRedis library
builder.Services.AddStackExchangeRedisCache(config =>
{   // Citanje ConnectionString za Redis iz appsettings
    config.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Add gRPC Service
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(config =>
{
    // U appsettings definisan localhost:5052( port za Discount) u GrpcSettings:DiscountUrl
    config.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    // Ovo je za TLS da ne kuka 
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

// Async communication between Basket and Ordering microservices
builder.Services.AddMessageBroker(builder.Configuration);
/* Basket je Publisher i zato AddMessageBroker se poziva samo sa builder.Configuration jer tad assembly = null */

/* Add Custom Exception Handler onaj iz BuildingBlocks kao u Catalog sto smo uradili
 Da u Postman lepsa poruka bude ako dodje do greske */
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure HTTP pipeline 

// Add MapCarter
app.MapCarter();

// Use Global Exception Handler in pipeline 
app.UseExceptionHandler(opts => { });

// Add Health check sa "/health" route u Postman 
//app.UseHealthChecks("/health",
//    new HealthCheckOptions
//    {
//        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//    }); ovo me nesto zeza pa sam ga markirao

app.Run();
