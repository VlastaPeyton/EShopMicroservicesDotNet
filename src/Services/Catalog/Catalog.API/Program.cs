// using u global using stavio
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Catalog.API.Data;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to Dependency Injection mora pre builder.Build.
builder.Services.AddCarter(); // For Minimal API Endpoint 
// Add Mediator
builder.Services.AddMediatR(config =>
{   // Registruje Handler klase
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    /*Dodam ValidationBehavior to MediatR pipeline from BuildingBlocks kako bi se iz BB
     povezao sa CommandValidator klasom u Handler. */
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    /* Dodam LoggingBehavior to MediatR pipeline from BuildingBlocks kako bi se iz BB
    povezao sa Command/QueryHandler klasom u Handler. */
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

// FluentValidation moram pre Marten da registrujem ja msm
// Mora sam FluentValidation.DependencyInjectionExtensions“  da instalim (u BuildingBlocks)
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMarten(config =>
{
    config.Connection(builder.Configuration.GetConnectionString("Database")!);
    // ! tells compiler result wont be null, supressing nullable reference type warning

}).UseLightweightSessions(); // LightWeightDocumentSession je najbolji kao sto znamo

// Seeding DB only for the first time if DB is empty only for Development, never in Production
// A development se bira u podesavanjima u VS 
if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

// Add custom exception handler definded in Bulding Blocks
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Add health check
builder.Services.AddHealthChecks() // da uradi healthcheck za Catalog.API
                .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);
                 // Da bi health check uradio i za catalogdb

var app = builder.Build();

// Configure HTTP request pipeline jer ovo moze samo na buildovanu applikaciju
app.MapCarter();

// Global Exception Handler in pipeline dodajem nakon sto sam ga addovao u builderu
app.UseExceptionHandler(options => { });

// Health check koji ima "/health" route u Postman 
app.UseHealthChecks("/health",
    // Da u Postman JSON format ovoga bude 
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
