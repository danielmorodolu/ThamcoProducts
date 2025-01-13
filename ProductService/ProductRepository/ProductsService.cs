using System;
using System.Net;
using Polly;
using Polly.Retry;
using Microsoft.Extensions.Logging;

namespace ProductService.ProductRepository
{

public class ProductsService : IProductService
{

    private readonly HttpClient _client;
    private readonly ILogger<ProductsService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    
        public ProductsService(HttpClient client, IConfiguration configuration, ILogger<ProductsService> logger)
       // public ProductsService(HttpClient client, IConfiguration configuration)
        {
            
            var baseUrl = configuration["WebServices:Products:BaseURL"] ?? "";
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
            _logger = logger;


            // Define Polly retry policy
        _retryPolicy = Policy.Handle<HttpRequestException>()
                             .Or<TaskCanceledException>() // Handle timeouts
                             .WaitAndRetryAsync(3, retryAttempt =>
                                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                                 (exception, timeSpan, retryCount, context) =>
                                 {
                                     _logger.LogWarning($"Retry {retryCount} after {timeSpan.Seconds} seconds due to {exception.Message}");
                                 });
}

public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var uri = "api/Product";
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Fetching products from {uri}...");
                var response = await _client.GetAsync(uri);

                response.EnsureSuccessStatusCode(); // Throw if status code is not successful
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                _logger.LogInformation($"Successfully fetched products from {uri}");

                return products ?? Array.Empty<ProductDto>();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching products from {uri}: {ex.Message}");
            return Array.Empty<ProductDto>(); // Fallback to an empty list
        }
    }
}
}