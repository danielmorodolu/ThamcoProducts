using System;
using System.Net;

namespace ProductService.ProductRepository;

public class ProductsService : IProductService
{

    private readonly HttpClient _client;
    

        public ProductsService(HttpClient client, IConfiguration configuration)
        {
            
            var baseUrl = configuration["WebServices:Products:BaseURL"] ?? "";
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var uri = "api/Product"; 
            try
            {
                var response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();  // Throws if not a successful status code
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                return products ?? Array.Empty<ProductDto>();
            }
            catch (Exception)
            {
                
                return Array.Empty<ProductDto>();  // Return an empty list if any exception occurs
            }
        }
}