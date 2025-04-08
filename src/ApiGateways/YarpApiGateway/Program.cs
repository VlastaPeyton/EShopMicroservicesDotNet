using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Register Yarp Reverse Proxy (Api Gateway) i ocitaj iz appsettings
builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// Add Rate Limiter za Yarp 
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
        // In 10s, max 5 request can be sent to application (Yar API Gateway)
    });
});
var app = builder.Build();

// Add Rate limiter
app.UseRateLimiter();

// Add Yarp Reverse Proxy in pipeline
app.MapReverseProxy();

app.Run();
