using Basket.API.Data;
using BuildingBlocks.Behaviours;
using BuildingBlocks.Exceptions;
using Discount.gRPC;
using HealthChecks.UI.Client;
using Marten;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;
using FluentValidation;
var builder = WebApplication.CreateBuilder(args);

// Add servicess to the container 

var assembly = typeof(Program).Assembly;

//Add Carter 
builder.Services.AddCarter(); // For Minimal API Endpoint

// Add MediatR
builder.Services.AddMediatR(config =>
{   // Registruje Handler klase jer registruje IRequestHandler koga implementira ICommand/IQueryHandler
    config.RegisterServicesFromAssemblies(assembly);
    //Dodam ValidationBehavior to MediatR pipeline from BuildingBlocks kako bi se iz BB povezao sa CommandValidator klasom u Handler.
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    //Dodam LoggingBehavior to MediatR pipeline from BuildingBlocks kako bi se iz BB povezao sa Command/QueryHandler u Handler.
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

// FluentValidation moram pre Marten da registrujem ja msm
// Morao sam FluentValidation.DependencyInjectionExtensions da instalim u BuildingBlocks pre ovoga
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly); // Finds CommandValidator klase da ValidationBehaviour moze da ih nadje

// Add Marten for Postgres NoSQL
builder.Services.AddMarten(config =>
{   // Citanje ConnectionStrings za bazu iz appsettings
    config.Connection(builder.Configuration.GetConnectionString("Database")!);
    // ShoppingCart.cs nema Id polje, pa uzimamo UserName polje za PK u tabeli koja je ShoppingCart tipa(jer NoSQL tabela nema ime). Moram imati PK polje kod NoSQL kao i kod SQL.
    config.Schema.For<ShoppingCart>().Identity(x => x.UserName);

}).UseLightweightSessions(); // Kao kod Catalog koristim LightWeighSession jer jee lagan, ali nema Change Tracker, pa moram da radim session.Store/Update before SaveChangesAsync ako elim promenu da sacuvam
 
// Add Repository as DI da Handler klase prepoznaju IBasketRepository kao BasketRepository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
/* Pored BasketRepository, moram da dodam i Redis cache. AKo uradim AddScoped<IBasketRepository, CachedBasketRepository>()
 onda cu da override ovo iznad. Ako ne koristim Scrutor, moram uraditi sledece da izbegnem override
  
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var basketRepository = provider.GetService<BasketRepository>();
    return new CachedBasketRepository(basketRepository, provider.GetRequiredServices<IDistributedCache>());
});

   Ali ovo nije dobro, jer ogroman kod. Vec radim with Scrutor library koja omogucava Decorator pattern ( linija ispod).
*/
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
// Ovo je Decorator pattern koji omoguacava da CachedBasketRepository koristi BacketRepository + da doda nesto svoje (Redis cache). Dodavanjem CachedBasketRepository kao Decorator za BasketRepository, u Handler klasama IBasketRepository predstavljace CachedBasketRepository.

// Add IDistributedCache from StackExchangeRedis library kako bih povezao Redis (in Docker) with this code.
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
// Basket je Publisher i zato AddMessageBroker se poziva samo sa builder.Configuration jer tad assembly = null 

// Add custom exception handler definded in Bulding Blocks to handle all unhandled exceptions accross Basket service, a svaki je unhandled jer nigde nemamo catch u funkcijama.
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
