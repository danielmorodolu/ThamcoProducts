using ProductService.ProductRepository;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using ThamcoProducts.ProductRepository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IProductService, ProductsService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = builder.Configuration["Auth:Domain"];
                        options.Audience = builder.Configuration["Auth:Audience"];
                    });
builder.Services.AddAuthorization();

if(builder.Environment.IsDevelopment()){
    builder.Services.AddSingleton<IProductService, FakeProductService>();
}
else {

   builder.Services.AddHttpClient<IProductService, ProductsService>()
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy()); ;
    
}


var app = builder.Build();

// Configure the HTTP request pipeline.
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

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
