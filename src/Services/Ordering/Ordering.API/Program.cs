using Ordering.API;
using Ordering.Infrastructure;
using Ordering.Application;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration)
                .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices(); // From Ordering.API DependencyInjection

// Samo za Development Seeding radimo 
if (app.Environment.IsDevelopment())
    await app.InitializeDatabaseAsync(); // Definisacu ovo 

app.Run();
