
using Discount.gRPC.Data;
using Discount.gRPC.Extensions;
using Discount.gRPC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
// Add gRPC Service (DiscountService.cs koja je nasledila discount.proto)

// Add DbContext 
builder.Services.AddDbContext<DiscountDbContext>(config =>
{   // Ocitavam connection string za SQLite iz appsettings
    config.UseSqlite(builder.Configuration.GetConnectionString("Database"));
});

var app = builder.Build();

app.UseMigration();  // AutoMigrate of SQL DB defined in Extensions.cs

app.MapGrpcService<DiscountService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
