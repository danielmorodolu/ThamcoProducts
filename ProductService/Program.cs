using ProductService.ProductRepository;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System.Net;
using ThamcoProducts.ProductRepository;

var builder = WebApplication.CreateBuilder(args);


// Create a logger
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

// Add HttpClient with policies for retry and circuit breaker
builder.Services.AddHttpClient<IProductService, ProductsService>()
    .AddPolicyHandler(GetRetryPolicy(logger))
    .AddPolicyHandler(GetCircuitBreakerPolicy(logger));

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductService, ProductsService>();

// Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Domain"];
        options.Audience = builder.Configuration["Auth:Audience"];
    });
builder.Services.AddAuthorization();

// Use a fake service in Development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IProductService, FakeProductService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

/// <summary>
/// Retry Policy for transient HTTP errors.
/// </summary>
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                // Log retry details
                var url = context.TryGetValue("RequestUri", out var uri) ? uri : "Unknown URL";
                logger.LogWarning(
                    "Retry {RetryCount} for {Url} failed with {StatusCode}. Retrying in {Delay}s.",
                    retryCount,
                    url,
                    outcome.Result?.StatusCode,
                    timespan.TotalSeconds
                );
            });
}

/// <summary>
/// Circuit Breaker Policy for handling repeated failures.
/// </summary>
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, breakDelay) =>
            {
                logger.LogError(
                    "Circuit opened for {Duration}s due to {StatusCode} or error: {Exception}",
                    breakDelay.TotalSeconds,
                    outcome.Result?.StatusCode,
                    outcome.Exception?.Message
                );
            },
            onReset: () =>
            {
                logger.LogInformation("Circuit closed. Normal operation resumed.");
            },
            onHalfOpen: () =>
            {
                logger.LogWarning("Circuit is half-open. Testing service health.");
            });
}
