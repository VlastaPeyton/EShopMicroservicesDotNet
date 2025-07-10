using Ordering.API;
using Ordering.Infrastructure;
using Ordering.Application;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration)    // From Ordering.Application DependencyInjection
                .AddInfrastructureServices(builder.Configuration) // From Ordering.Infrastrucutre DependencyInjection
                .AddApiServices(builder.Configuration);           // From Ordering.API DependencyInjection

var app = builder.Build();

app.UseApiServices(); // From Ordering.API DependencyInjection

// Samo za Development Seeding radimo 
if (app.Environment.IsDevelopment())
    await app.InitializeDatabaseAsync(); // Definisan ovaj extension u Infrastructure layer.

app.Run();
