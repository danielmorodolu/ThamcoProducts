using System;
using System.Threading.Tasks;
using ProductService.ProductRepository;

namespace ThamcoProducts.ProductRepository;

public class FakeProductService : IProductService
{
    private readonly ProductDto[] _products =
    {
        new ProductDto { Id = 1, Name = "Fake product A" },
        new ProductDto { Id = 2, Name = "Fake product B" },
        new ProductDto { Id = 3, Name = "Fake product C" }
    };

    private int _failureCount = 0; // Track how many times transient errors occur
    private readonly int _maxFailuresBeforeSuccess;

    public FakeProductService(int maxFailuresBeforeSuccess = 3)
    {
        _maxFailuresBeforeSuccess = maxFailuresBeforeSuccess;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        // Simulate transient errors
        if (_failureCount < _maxFailuresBeforeSuccess)
        {
            _failureCount++;
            throw new HttpRequestException($"Simulated transient error #{_failureCount}");
        }

        // If no errors, return fake data
        return await Task.FromResult(_products.AsEnumerable());
    }
}
